using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Invento.API.Swagger
{
    public class AuthorizeOperationFilter
        : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
            var endpointMetadata =
                context.ApiDescription
                    .ActionDescriptor
                    .EndpointMetadata;

            var allowsAnonymous =
                endpointMetadata
                    .OfType<IAllowAnonymous>()
                    .Any();

            if (allowsAnonymous)
            {
                return;
            }

            var requiresAuthorization =
                endpointMetadata
                    .OfType<IAuthorizeData>()
                    .Any();

            if (!requiresAuthorization)
            {
                return;
            }

            operation.Security ??=
                new List<OpenApiSecurityRequirement>();

            operation.Security.Add(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =
                                new OpenApiReference
                                {
                                    Type =
                                        ReferenceType
                                            .SecurityScheme,

                                    Id = "Bearer"
                                }
                        },
                        Array.Empty<string>()
                    }
                });

            if (!operation.Responses.ContainsKey("401"))
            {
                operation.Responses.Add(
                    "401",
                    new OpenApiResponse
                    {
                        Description =
                            "Unauthorized. A valid access token is required."
                    });
            }

            var authorizeData =
                endpointMetadata
                    .OfType<IAuthorizeData>()
                    .ToList();

            var hasRoleOrPolicyRequirement =
                authorizeData.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Roles)
                    ||
                    !string.IsNullOrWhiteSpace(x.Policy));

            if (hasRoleOrPolicyRequirement &&
                !operation.Responses.ContainsKey("403"))
            {
                operation.Responses.Add(
                    "403",
                    new OpenApiResponse
                    {
                        Description =
                            "Forbidden. The authenticated user does not have sufficient permission."
                    });
            }
        }
    }
}