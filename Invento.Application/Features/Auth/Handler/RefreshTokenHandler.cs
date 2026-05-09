using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Data;
using Invento.Application.Features.Auth.Models;
using Invento.Domain.Entities;
using MediatR;
using System.Data;

namespace Invento.Application.Features.Auth.Handler
{
    public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IDbConnectionFactory _db;
        private readonly IJwtService _jwt;

        public RefreshTokenHandler(IDbConnectionFactory db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                    throw new UnauthorizedAccessException("Invalid refresh token");

                var tokenHash = RefreshTokenService.Hash(request.RefreshToken);

                var token = await connection.QueryFirstOrDefaultAsync<RefreshToken>(
                    @"SELECT Id, UserId, TokenHash, ExpiresAt, IsRevoked
                      FROM RefreshTokens
                      WHERE TokenHash = @TokenHash AND IsRevoked = 0",
                    new { TokenHash = tokenHash },
                    transaction);

                if (token == null || token.ExpiresAt <= DateTime.UtcNow)
                    throw new UnauthorizedAccessException("Invalid refresh token");

                await connection.ExecuteAsync(
                    @"UPDATE RefreshTokens 
                      SET IsRevoked = 1 
                      WHERE Id = @Id",
                    new { token.Id },
                    transaction);

                var user = await connection.QueryFirstOrDefaultAsync<User>(
                    @"SELECT Id, TenantId, Email, Role
                      FROM Users
                      WHERE Id = @UserId",
                    new { token.UserId },
                    transaction);

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid user");

                var newAccessToken = _jwt.GenerateToken(
                    user.Id,
                    user.TenantId,
                    user.Role,
                    user.Email);

                var newRefreshToken = RefreshTokenService.GenerateToken();
                var newRefreshHash = RefreshTokenService.Hash(newRefreshToken);

                await connection.ExecuteAsync(
                    @"INSERT INTO RefreshTokens 
                      (Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt)
                      VALUES 
                      (@Id, @UserId, @TokenHash, @ExpiresAt, 0, GETUTCDATE())",
                    new
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TokenHash = newRefreshHash,
                        ExpiresAt = DateTime.UtcNow.AddDays(7)
                    },
                    transaction);

                transaction.Commit();

                return new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}