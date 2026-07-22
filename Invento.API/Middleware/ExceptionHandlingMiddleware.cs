using FluentValidation;
using Invento.API.Common;
using Invento.Shared.Exceptions;
using System.Net;
using System.Text.Json;

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
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found");

                await WriteResponse(
                    context,
                    HttpStatusCode.NotFound,
                    new ErrorResponse
                    {
                        Message = ex.Message
                    });
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Validation error occurred");

                var response =
                    new ErrorResponse
                    {
                        Message = "Validation failed",
                        Errors = ex.Errors
                            .Select(x => x.ErrorMessage)
                            .ToList()
                    };

                await WriteResponse(
                    context,
                    HttpStatusCode.BadRequest,
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unhandled exception occurred");

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

                var response =
                    new ErrorResponse
                    {
                        Message = "Internal server error",
                        Errors = errors
                    };

                await WriteResponse(
                    context,
                    HttpStatusCode.InternalServerError,
                    response);
            }
        }

        private static async Task WriteResponse(
            HttpContext context,
            HttpStatusCode statusCode,
            ErrorResponse response)
        {
            context.Response.StatusCode =
                (int)statusCode;

            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response));
        }
    }
}