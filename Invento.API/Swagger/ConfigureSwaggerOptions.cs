using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

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
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "Invento API",
                        Version = description.ApiVersion.ToString(),
                        Description =
                            "Invento ERP API"
                    });
            }

            var securityScheme =
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",

                    Description =
                        "Enter JWT Bearer Token",

                    In = ParameterLocation.Header,

                    Type = SecuritySchemeType.Http,

                    Scheme = "bearer",

                    BearerFormat = "JWT",

                    Reference =
                        new OpenApiReference
                        {
                            Id = "Bearer",
                            Type =
                                ReferenceType.SecurityScheme
                        }
                };

            options.AddSecurityDefinition(
                "Bearer",
                securityScheme);

            options.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        securityScheme,
                        Array.Empty<string>()
                    }
                });
        }
    }
}