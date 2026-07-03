using FluentValidation;
using Invento.API.Common;
using System.Net;
using System.Text.Json;

namespace Invento.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private const string CorrelationIdItem =
            "CorrelationId";

        private const string CorrelationIdHeader =
            "X-Correlation-ID";

        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionHandlingMiddleware>
            _logger;

        private readonly IWebHostEnvironment _environment;

        private static readonly JsonSerializerOptions
            JsonOptions = new()
            {
                PropertyNamingPolicy =
                    JsonNamingPolicy.CamelCase
            };

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
                await HandleValidationExceptionAsync(
                    context,
                    ex);
            }
            catch (OperationCanceledException ex)
                when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation(
                    ex,
                    "HTTP request was cancelled by the client. " +
                    "Method: {Method}, Path: {Path}",
                    context.Request.Method,
                    context.Request.Path);

            }
            catch (Exception ex)
            {
                await HandleUnhandledExceptionAsync(
                    context,
                    ex);
            }
        }

        private async Task HandleValidationExceptionAsync(
            HttpContext context,
            ValidationException exception)
        {
            var correlationId =
                GetCorrelationId(context);

            _logger.LogWarning(
                exception,
                "Validation failed for HTTP request. " +
                "Method: {Method}, Path: {Path}, " +
                "CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning(
                    "Cannot write validation error response " +
                    "because the HTTP response has already started. " +
                    "CorrelationId: {CorrelationId}",
                    correlationId);

                return;
            }

            context.Response.Clear();

            context.Response.StatusCode =
                (int)HttpStatusCode.BadRequest;

            context.Response.ContentType =
                "application/json";

            SetCorrelationIdHeader(
                context,
                correlationId);

            var errors =
                exception.Errors
                    .Select(error =>
                        error.ErrorMessage)
                    .Where(message =>
                        !string.IsNullOrWhiteSpace(message))
                    .Distinct()
                    .ToList();

            var response =
                new ErrorResponse
                {
                    Message =
                        "Validation failed",

                    Errors =
                        errors
                };

            await WriteResponseAsync(
                context,
                response);
        }

        private async Task HandleUnhandledExceptionAsync(
            HttpContext context,
            Exception exception)
        {
            var correlationId =
                GetCorrelationId(context);

            _logger.LogError(
                exception,
                "Unhandled exception occurred while " +
                "processing HTTP request. " +
                "Method: {Method}, Path: {Path}, " +
                "CorrelationId: {CorrelationId}",
                context.Request.Method,
                context.Request.Path,
                correlationId);

            if (context.Response.HasStarted)
            {
                _logger.LogWarning(
                    "Cannot write internal server error response " +
                    "because the HTTP response has already started. " +
                    "CorrelationId: {CorrelationId}",
                    correlationId);

                return;
            }

            context.Response.Clear();

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            context.Response.ContentType =
                "application/json";

            SetCorrelationIdHeader(
                context,
                correlationId);

            var response =
                new ErrorResponse
                {
                    Message =
                        "Internal server error",

                    Errors =
                        _environment.IsDevelopment()
                            ? new List<string>
                            {
                                exception.Message
                            }
                            : new List<string>
                            {
                                "An unexpected error occurred."
                            }
                };

            await WriteResponseAsync(
                context,
                response);
        }

        private static string GetCorrelationId(
            HttpContext context)
        {
            if (context.Items.TryGetValue(
                    CorrelationIdItem,
                    out var value) &&
                value is string correlationId &&
                !string.IsNullOrWhiteSpace(
                    correlationId))
            {
                return correlationId;
            }

            var incomingCorrelationId =
                context.Request.Headers[
                    CorrelationIdHeader]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(
                incomingCorrelationId))
            {
                return incomingCorrelationId.Trim();
            }

            return context.TraceIdentifier;
        }

        private static void SetCorrelationIdHeader(
            HttpContext context,
            string correlationId)
        {
            context.Response.Headers[
                CorrelationIdHeader] =
                correlationId;
        }

        private static async Task WriteResponseAsync(
            HttpContext context,
            ErrorResponse response)
        {
            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    response,
                    JsonOptions));
        }
    }
}