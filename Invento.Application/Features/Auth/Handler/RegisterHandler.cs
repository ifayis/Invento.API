using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Data;
using Invento.Application.Features.Auth.Commands;
using Invento.Application.Features.Auth.Models;
using Invento.Domain.Entities;
using MediatR;
using System.Data;

namespace Invento.Application.Features.Auth.Handler
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IDbConnectionFactory _db;
        private readonly IJwtService _jwt;

        public RegisterHandler(IDbConnectionFactory db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            using var connection = _db.CreateConnection();

            if (connection.State != ConnectionState.Open)
                connection.Open();

            using var transaction = connection.BeginTransaction();

            try
            {
                var email = request.Email.Trim().ToLower();
                var companyCode = request.CompanyCode.Trim().ToUpper();

                var tenant = await connection.QueryFirstOrDefaultAsync<Tenant>(
                    @"SELECT Id, Name, CompanyCode 
                      FROM Tenants 
                      WHERE CompanyCode = @CompanyCode",
                    new { CompanyCode = companyCode },
                    transaction);

                Guid tenantId;

                if (tenant == null)
                {
                    tenantId = Guid.NewGuid();

                    await connection.ExecuteAsync(
                        @"INSERT INTO Tenants 
                          (Id, Name, CompanyCode, BusinessPurpose, LogoUrl, CreatedAt)
                          VALUES 
                          (@Id, @Name, @CompanyCode, @BusinessPurpose, @LogoUrl, GETUTCDATE())",
                        new
                        {
                            Id = tenantId,
                            Name = request.CompanyName,
                            CompanyCode = companyCode,
                            request.BusinessPurpose,
                            request.LogoUrl
                        },
                        transaction);
                }
                else
                {
                    tenantId = tenant.Id;
                }

                var exists = await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) FROM Users WHERE Email = @Email",
                    new { Email = email },
                    transaction);

                if (exists > 0)
                    throw new InvalidOperationException("User already exists");

                var userId = Guid.NewGuid();
                var passwordHash = PasswordHasher.Hash(request.Password);

                await connection.ExecuteAsync(
                    @"INSERT INTO Users 
                      (Id, TenantId, Email, PasswordHash, Role, CreatedAt)
                      VALUES 
                      (@Id, @TenantId, @Email, @PasswordHash, @Role, GETUTCDATE())",
                    new
                    {
                        Id = userId,
                        TenantId = tenantId,
                        Email = email,
                        PasswordHash = passwordHash,
                        Role = "Admin"
                    },
                    transaction);

                var accessToken = _jwt.GenerateToken(userId, tenantId, "Admin", email);

                var refreshToken = RefreshTokenService.GenerateToken();
                var refreshHash = RefreshTokenService.Hash(refreshToken);

                await connection.ExecuteAsync(
                    @"INSERT INTO RefreshTokens 
                      (Id, UserId, TokenHash, ExpiresAt, IsRevoked, CreatedAt)
                      VALUES 
                      (@Id, @UserId, @TokenHash, @ExpiresAt, 0, GETUTCDATE())",
                    new
                    {
                        Id = Guid.NewGuid(),
                        UserId = userId,
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