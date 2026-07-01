using System.Reflection;
using System.Text;
using System.Security.Cryptography;

namespace Invento.Application.Common.Caching
{
    public static class CacheKeyBuilder
    {
        public static string Build<T>(
            T request)
        {
            if (request is null)
            {
                return "default";
            }

            var properties =
                typeof(T)
                    .GetProperties(
                        BindingFlags.Public |
                        BindingFlags.Instance)
                    .OrderBy(x => x.Name);

            var builder = new StringBuilder();

            foreach (var property in properties)
            {
                var value =
                    property.GetValue(request);

                if (value is null)
                {
                    continue;
                }

                builder.Append(
                    property.Name);

                builder.Append('=');

                builder.Append(value);

                builder.Append(';');
            }

            var rawKey = builder.ToString();

            var hash =
                Convert.ToHexString(
                    SHA256.HashData(
                        Encoding.UTF8.GetBytes(rawKey))
                );

            return hash;
        }
    }
}