using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Invento.API.Swagger
{
    public class StandardResponseOperationFilter
        : IOperationFilter
    {
        public void Apply(
            OpenApiOperation operation,
            OperationFilterContext context)
        {
            AddResponseIfMissing(
                operation,
                "400",
                "Bad Request. The request is invalid or validation failed.");

            AddResponseIfMissing(
                operation,
                "429",
                "Too Many Requests. The configured rate limit was exceeded.");

            AddResponseIfMissing(
                operation,
                "500",
                "Internal Server Error. An unexpected server error occurred.");

            var httpMethod =
                context.ApiDescription
                    .HttpMethod?
                    .ToUpperInvariant();

            if (httpMethod is
                "POST" or
                "PUT" or
                "PATCH" or
                "DELETE")
            {
                AddResponseIfMissing(
                    operation,
                    "409",
                    "Conflict. The request conflicts with the current resource state.");
            }
        }

        private static void AddResponseIfMissing(
            OpenApiOperation operation,
            string statusCode,
            string description)
        {
            if (operation.Responses.ContainsKey(
                statusCode))
            {
                return;
            }

            operation.Responses.Add(
                statusCode,
                new OpenApiResponse
                {
                    Description = description
                });
        }
    }
}