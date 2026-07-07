using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Invento.API.Swagger
{
    public class ConfigureSwaggerOptions
        : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(
            SwaggerGenOptions options)
        {
            foreach (var description in
                _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "Invento API",

                        Version =
                            description.ApiVersion
                                .ToString(),

                        Description =
                            """
                            Invento is a multi-tenant business management API
                            for inventory, purchasing, sales, customers,
                            suppliers, payments, stock tracking and analytics.

                            Authentication uses JWT access tokens and rotating
                            refresh tokens.

                            Tenant isolation is enforced by the authenticated
                            tenant context.

                            Use the Authorize button for protected endpoints.
                            Enter the JWT access token only; Swagger adds the
                            Bearer scheme automatically.
                            """,

                        Contact =
                            new OpenApiContact
                            {
                                Name = "Invento API Support"
                            }
                    });
            }

            options.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",

                    Description =
                        "JWT Bearer authentication. " +
                        "Enter the access token only.",

                    In = ParameterLocation.Header,

                    Type = SecuritySchemeType.Http,

                    Scheme = "bearer",

                    BearerFormat = "JWT"
                });

            options.OperationFilter<
                AuthorizeOperationFilter>();

            options.OperationFilter<
                StandardResponseOperationFilter>();

            options.SchemaFilter<
                SwaggerExampleSchemaFilter>();

            var xmlFileName =
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlFilePath =
                Path.Combine(
                    AppContext.BaseDirectory,
                    xmlFileName);

            if (File.Exists(xmlFilePath))
            {
                options.IncludeXmlComments(
                    xmlFilePath,
                    includeControllerXmlComments: true);
            }

            options.CustomSchemaIds(
                type =>
                    type.FullName?
                        .Replace("+", ".")
                    ?? type.Name);
        }
    }
}