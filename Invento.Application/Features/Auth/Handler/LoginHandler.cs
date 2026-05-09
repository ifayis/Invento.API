using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Features.Auth.Commands;
using Invento.Application.Features.Auth.Models;
using Invento.Domain.Entities;
using MediatR;
using System.Data;

namespace Invento.Application.Features.Auth.Handler
{
    public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IDbConnectionFactory _db;
        private readonly IJwtService _jwt;

        public LoginHandler(IDbConnectionFactory db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var email = request.Email.Trim().ToLower();

                var user = await connection.QueryFirstOrDefaultAsync<User>(
                    @"SELECT Id, TenantId, Email, PasswordHash, Role
                      FROM Users 
                      WHERE Email = @Email",
                    new { Email = email },
                    transaction);

                if (user == null)
                    throw new UnauthorizedAccessException("Invalid credentials");

                var isValid = PasswordHasher.Verify(request.Password, user.PasswordHash);

                if (!isValid)
                    throw new UnauthorizedAccessException("Invalid credentials");

                var accessToken = _jwt.GenerateToken(
                    user.Id,
                    user.TenantId,
                    user.Role,
                    user.Email
                );

                var refreshToken = RefreshTokenService.GenerateToken();
                var refreshHash = RefreshTokenService.Hash(refreshToken);

                await connection.ExecuteAsync(
                    @"INSERT INTO RefreshToken
                      (Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt)
                      VALUES 
                      (@Id, @UserId, @TokenHash, @ExpiresAt, 0, GETUTCDATE())",
                    new
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TokenHash = refreshHash,
                        ExpiresAt = DateTime.UtcNow.AddDays(7)
                    },
                    transaction);

                transaction.Commit();

                return new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
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