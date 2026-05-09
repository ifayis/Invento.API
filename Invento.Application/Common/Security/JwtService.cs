using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Invento.Application.Common.Interface;
using Microsoft.Extensions.Configuration;

namespace Invento.Application.Common.Security
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(Guid userId, Guid tenantId, string role, string email)
        {
            var claims = new[]
            {
                new Claim("UserId", userId.ToString()),
                new Claim("TenantId",tenantId.ToString()),
                new Claim("Role", role),
                new Claim("Email", email)
            };

            var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
