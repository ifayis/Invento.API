using Dapper;
using MediatR;
using Invento.Application.Common.Interface;
using Invento.Application.Common.Security;
using Invento.Application.Features.Auth.Commands;
using Invento.Domain.Entities;
using Invento.Application.Data;


namespace Invento.Application.Features.Auth.Handler
{
    public class LoginHandler : IRequestHandler<LoginCommand, string>
    {
        private readonly IDbConnectionFactory _db;
        private readonly IJwtService _jwt;

        public LoginHandler(IDbConnectionFactory db, IJwtService jwt)
        {
            _db = db;
            _jwt = jwt;
        }

        public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var connection = _db.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT Id, TenantId, Email, PasswordHash, Role
                  FROM Users 
                  WHERE Email = @Email",
                new { request.Email });

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var isValid = PasswordHasher.Verify(request.Password, user.PasswordHash);

            if (!isValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            return _jwt.GenerateToken(
                user.Id,
                user.TenantId,
                user.Role,
                user.Email
            );
        }
    }
}