using Invento.Application.Common.Interface;
using Invento.Application.Common.Secuirity;
using Invento.Application.Data;
using MediatR;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Auth.Commands
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

            var user = await connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT * FROM Users WHERE Email=@Email",
                new { request.Email });

            if (user == null)
                throw new Exception("Invalid credentials");

            if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
                throw new Exception("Invalid credentials");

            return _jwt.GenerateToken(user.Id, user.TenantId, user.Role, user.Email);
        }
    }
}
