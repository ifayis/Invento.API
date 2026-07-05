using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Invento.Persistence.Auditing
{
    public static class AuditValueSerializer
    {
        private static readonly HashSet<string>
            SensitivePropertyNames =
                new(StringComparer.OrdinalIgnoreCase)
                {
                    "Password",
                    "PasswordHash",
                    "CurrentPassword",
                    "NewPassword",
                    "ConfirmPassword",
                    "Token",
                    "TokenHash",
                    "RefreshToken",
                    "AccessToken",
                    "Secret",
                    "SecretKey",
                    "ApiKey"
                };

        public static string? GetOldValues(
            EntityEntry entry)
        {
            var values =
                new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (IsSensitive(
                    property.Metadata.Name))
                {
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    continue;
                }

                if (!property.IsModified)
                {
                    continue;
                }

                values[property.Metadata.Name] =
                    property.OriginalValue;
            }

            return SerializeOrNull(values);
        }

        public static string? GetNewValues(EntityEntry entry)
        {
            var values =
                new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (IsSensitive(
                    property.Metadata.Name))
                {
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    continue;
                }

                if (!property.IsModified)
                {
                    continue;
                }

                values[property.Metadata.Name] =
                    property.CurrentValue;
            }

            return SerializeOrNull(values);
        }

        public static string? GetCreatedValues(
            EntityEntry entry)
        {
            var values =
                new Dictionary<string, object?>();

            foreach (var property in entry.Properties)
            {
                if (IsSensitive(
                    property.Metadata.Name))
                {
                    continue;
                }

                if (property.Metadata.IsPrimaryKey())
                {
                    continue;
                }

                values[property.Metadata.Name] =
                    property.CurrentValue;
            }

            return SerializeOrNull(values);
        }

        private static bool IsSensitive(
            string propertyName)
        {
            return SensitivePropertyNames.Contains(
                propertyName);
        }

        private static string? SerializeOrNull(
            Dictionary<string, object?> values)
        {
            if (values.Count == 0)
            {
                return null;
            }

            return JsonSerializer.Serialize(values);
        }
    }
}