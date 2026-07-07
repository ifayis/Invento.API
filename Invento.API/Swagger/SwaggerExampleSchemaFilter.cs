using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Invento.API.Swagger
{
    public class SwaggerExampleSchemaFilter
        : ISchemaFilter
    {
        public void Apply(
            OpenApiSchema schema,
            SchemaFilterContext context)
        {
            if (schema.Properties is null ||
                schema.Properties.Count == 0)
            {
                return;
            }

            foreach (var property in schema.Properties)
            {
                if (property.Value.Example is not null)
                {
                    continue;
                }

                property.Value.Example =
                    CreateExample(
                        property.Key,
                        property.Value);
            }
        }

        private static IOpenApiAny? CreateExample(
            string propertyName,
            OpenApiSchema schema)
        {
            var normalizedName =
                propertyName.ToLowerInvariant();

            if (normalizedName.Contains("email"))
            {
                return new OpenApiString(
                    "user@example.com");
            }

            if (normalizedName.Contains("password"))
            {
                return new OpenApiString(
                    "StrongPassword123!");
            }

            if (normalizedName == "fullname" ||
                normalizedName == "name")
            {
                return new OpenApiString(
                    "Example Name");
            }

            if (normalizedName.Contains("sku"))
            {
                return new OpenApiString(
                    "SKU-001");
            }

            if (normalizedName.Contains("phone"))
            {
                return new OpenApiString(
                    "+919876543210");
            }

            if (normalizedName.EndsWith("id") &&
                schema.Type == "string")
            {
                return new OpenApiString(
                    "11111111-1111-1111-1111-111111111111");
            }

            if (schema.Format == "uuid")
            {
                return new OpenApiString(
                    "11111111-1111-1111-1111-111111111111");
            }

            if (schema.Format == "date-time")
            {
                return new OpenApiString(
                    "2026-07-07T12:00:00Z");
            }

            return schema.Type switch
            {
                "string" =>
                    new OpenApiString(
                        "Example value"),

                "integer" =>
                    new OpenApiInteger(1),

                "number" =>
                    new OpenApiDouble(100.00),

                "boolean" =>
                    new OpenApiBoolean(true),

                _ => null
            };
        }
    }
}