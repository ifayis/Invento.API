using Invento.API.Common;
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Invento.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<
            ExceptionHandlingMiddleware> _logger;

        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Validation error occurred");

                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                var response = new ErrorResponse
                {
                    Message = "Validation failed",

                    Errors = ex.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList()
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred");

                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                var errors =
                    _environment.IsDevelopment()
                        ? new List<string>
                        {
                            ex.Message
                        }
                        : new List<string>
                        {
                            "An unexpected error occurred"
                        };

                var response = new ErrorResponse
                {
                    Message = "Internal server error",

                    Errors = errors
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(response));
            }
        }
    }
}