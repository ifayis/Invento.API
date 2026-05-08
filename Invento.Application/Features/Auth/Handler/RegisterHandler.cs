using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Data;
using Invento.Application.Features.Auth.Commands;
using Invento.Domain.Entities;
using MediatR;

public class RegisterHandler : IRequestHandler<RegisterCommand, string>
{
    private readonly IDbConnectionFactory _db;
    private readonly IJwtService _jwt;

    public RegisterHandler(IDbConnectionFactory db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        using var connection = _db.CreateConnection();
        connection.Open();

        using var transaction = connection.BeginTransaction();

        var tenant = await connection.QueryFirstOrDefaultAsync<Tenant>(
            @"SELECT * FROM Tenants WHERE CompanyCode = @CompanyCode",
            new { request.CompanyCode }, transaction);

        Guid tenantId;

        if (tenant == null)
        {
            tenantId = Guid.NewGuid();

            await connection.ExecuteAsync(
                @"INSERT INTO Tenants (Id, Name, CompanyCode, CreatedAt)
              VALUES (@Id, @Name, @CompanyCode, GETUTCDATE())",
                new
                {
                    Id = tenantId,
                    Name = request.CompanyName,
                    CompanyCode = request.CompanyCode
                }, transaction);
        }
        else
        {
            tenantId = tenant.Id;
        }

        var exists = await connection.ExecuteScalarAsync<int>(
            @"SELECT COUNT(1) FROM Users WHERE Email = @Email",
            new { request.Email }, transaction);

        if (exists > 0)
            throw new Exception("User already exists");

        var userId = Guid.NewGuid();
        var hash = PasswordHasher.Hash(request.Password);

        await connection.ExecuteAsync(
            @"INSERT INTO Users 
          (Id, TenantId, Email, PasswordHash, Role, CreatedAt)
          VALUES 
          (@Id, @TenantId, @Email, @PasswordHash, @Role, GETUTCDATE())",
            new
            {
                Id = userId,
                TenantId = tenantId,
                Email = request.Email,
                PasswordHash = hash,
                Role = "Admin"
            }, transaction);

        transaction.Commit();

        return _jwt.GenerateToken(userId, tenantId, "Admin", request.Email);
    }
}