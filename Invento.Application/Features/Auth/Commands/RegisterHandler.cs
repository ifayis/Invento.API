using Dapper;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Secuirity;
using Invento.Application.Data;
using Invento.Application.Features.Auth.Commands;
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
        var connection = _db.CreateConnection();

        if (string.IsNullOrWhiteSpace(request.Email))
            throw new Exception("Email required");

        if (request.Password.Length < 6)
            throw new Exception("Password too short");

        var tenant = await connection.QueryFirstOrDefaultAsync<dynamic>(
            "SELECT * FROM Tenants WHERE CompanyCode = @Code",
            new { Code = request.CompanyCode });

        Guid tenantId;

        if (tenant == null)
        {
            tenantId = Guid.NewGuid();

            await connection.ExecuteAsync(
                "INSERT INTO Tenants (Id, Name, CompanyCode, CreatedAt) VALUES (@Id,@Name,@Code,GETUTCDATE())",
                new { Id = tenantId, Name = request.CompanyName, Code = request.CompanyCode });
        }
        else
        {
            tenantId = tenant.Id;
        }

        var exists = await connection.ExecuteScalarAsync<int>(
            "SELECT COUNT(1) FROM Users WHERE Email=@Email",
            new { request.Email });

        if (exists > 0)
            throw new Exception("User already exists");

        var userId = Guid.NewGuid();

        var hash = PasswordHasher.Hash(request.Password);

        await connection.ExecuteAsync(
            @"INSERT INTO Users (Id, TenantId, Email, PasswordHash, Role, CreatedAt)
              VALUES (@Id,@TenantId,@Email,@Hash,'Admin',GETUTCDATE())",
            new
            {
                Id = userId,
                TenantId = tenantId,
                request.Email,
                Hash = hash
            });

        return _jwt.GenerateToken(userId, tenantId, "Admin", request.Email);
    }
}