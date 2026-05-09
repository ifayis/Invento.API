using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Features.Auth.Models;
using Invento.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Invento.Application.Features.Auth.Handler
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IDbConnectionFactory _db;
        private readonly IJwtService _jwt;
        private readonly ILogger<RefreshTokenHandler> _logger;

        public RefreshTokenHandler(
            IDbConnectionFactory db,
            IJwtService jwt,
            ILogger<RefreshTokenHandler> logger)
        {
            _db = db;
            _jwt = jwt;
            _logger = logger;
        }

        public async Task<AuthResponse> Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    _logger.LogWarning(
                        "Refresh token request failed because token was empty");

                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                var tokenHash = RefreshTokenService.Hash(request.RefreshToken);

                var token = await connection.QueryFirstOrDefaultAsync<RefreshToken>(
                    new CommandDefinition(
                        commandText:
                        @"SELECT Id,
                                 UserId,
                                 TokenHash,
                                 ExpiresAt,
                                 IsRevoked,
                                 CreatedAt
                          FROM RefreshToken
                          WHERE TokenHash = @TokenHash
                          AND IsRevoked = 0",
                        parameters: new
                        {
                            TokenHash = tokenHash
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken
                    ));

                if (token == null || token.ExpiresAt <= DateTime.UtcNow)
                {
                    _logger.LogWarning(
                        "Invalid or expired refresh token attempt");

                    throw new UnauthorizedAccessException("Invalid refresh token");
                }

                var affected = await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText:
                        @"UPDATE RefreshToken
                          SET IsRevoked = 1
                          WHERE Id = @Id
                          AND IsRevoked = 0",
                        parameters: new
                        {
                            token.Id
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken
                    ));

                if (affected == 0)
                {
                    _logger.LogWarning(
                        "Refresh token replay detected for UserId: {UserId}",
                        token.UserId);

                    throw new UnauthorizedAccessException(
                        "Refresh token already used");
                }

                var user = await connection.QueryFirstOrDefaultAsync<User>(
                    new CommandDefinition(
                        commandText:
                        @"SELECT Id,
                                 TenantId,
                                 Email,
                                 Role
                          FROM Users
                          WHERE Id = @UserId",
                        parameters: new
                        {
                            token.UserId
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken
                    ));

                if (user == null)
                {
                    _logger.LogWarning(
                        "Refresh token user not found. UserId: {UserId}",
                        token.UserId);

                    throw new UnauthorizedAccessException("Invalid user");
                }

                var newAccessToken = _jwt.GenerateToken(
                    user.Id,
                    user.TenantId,
                    user.Role,
                    user.Email);

                var newRefreshToken = RefreshTokenService.GenerateToken();

                var newRefreshHash =
                    RefreshTokenService.Hash(newRefreshToken);

                await connection.ExecuteAsync(
                    new CommandDefinition(
                        commandText:
                        @"INSERT INTO RefreshToken
                          (
                              Id,
                              UserId,
                              TokenHash,
                              ExpiresAt,
                              IsRevoked,
                              CreatedAt
                          )
                          VALUES
                          (
                              @Id,
                              @UserId,
                              @TokenHash,
                              @ExpiresAt,
                              0,
                              GETUTCDATE()
                          )",
                        parameters: new
                        {
                            Id = Guid.NewGuid(),
                            UserId = user.Id,
                            TokenHash = newRefreshHash,
                            ExpiresAt = DateTime.UtcNow.AddDays(7)
                        },
                        transaction: transaction,
                        cancellationToken: cancellationToken
                    ));

                transaction.Commit();

                _logger.LogInformation(
                    "Refresh token rotated successfully for UserId: {UserId}",
                    user.Id);

                return new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch (Exception ex)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(
                        rollbackEx,
                        "Transaction rollback failed during refresh token handling");
                }

                _logger.LogError(
                    ex,
                    "Refresh token flow failed");

                throw;
            }
        }
    }
}