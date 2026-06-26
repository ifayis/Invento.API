using System.Security.Cryptography;

namespace Invento.Application.Common
{
    public static class PasswordGenerator
    {
        private const string Upper =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const string Lower =
            "abcdefghijklmnopqrstuvwxyz";

        private const string Numbers =
            "0123456789";

        private const string Special =
            "!@#$%^&*";

        private static readonly string All =
            Upper + Lower + Numbers + Special;

        public static string Generate(
            int length = 12)
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(length, 8);

            var chars = new char[length];

            chars[0] = Upper[RandomNumberGenerator.GetInt32(Upper.Length)];
            chars[1] = Lower[RandomNumberGenerator.GetInt32(Lower.Length)];
            chars[2] = Numbers[RandomNumberGenerator.GetInt32(Numbers.Length)];
            chars[3] = Special[RandomNumberGenerator.GetInt32(Special.Length)];

            for (var i = 4; i < length; i++)
            {
                chars[i] =
                    All[RandomNumberGenerator.GetInt32(All.Length)];
            }

            for (var i = chars.Length - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);

                (chars[i], chars[j]) =
                    (chars[j], chars[i]);
            }

            return new string(chars);
        }
    }
}