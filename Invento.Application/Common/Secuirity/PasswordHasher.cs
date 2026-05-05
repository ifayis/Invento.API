using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Common.Secuirity
{
    public static class PasswordHasher
    {
        public static string Hash(string Password)
        {
            return BCrypt.Net.BCrypt.HashPassword(Password);
        }

        public static bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
