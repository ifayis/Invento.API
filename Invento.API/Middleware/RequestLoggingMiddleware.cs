using System.Diagnostics;
using Invento.Application.Interfaces;
using Serilog.Context;

namespace Invento.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private const string CorrelationIdHeader =
            "X-Correlation-ID";

        private const string CorrelationIdItem =
            "CorrelationId";

        private readonly RequestDelegate _next;

        private readonly ILogger<RequestLoggingMiddleware>
            _logger;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(
            HttpContext context,
            ICurrentTenantService currentTenant,
            ICurrentUserService currentUser)
        {
            var stopwatch =
                Stopwatch.StartNew();

            var correlationId =
                GetOrCreateCorrelationId(context);

            context.Items[CorrelationIdItem] =
                correlationId;

            context.Response.OnStarting(() =>
            {
                context.Response.Headers[
                    CorrelationIdHeader] =
                    correlationId;

                return Task.CompletedTask;
            });

            var userId =
                context.User.Identity?.IsAuthenticated == true
                    ? currentUser.UserId
                    : null;

            var tenantId =
                context.User.Identity?.IsAuthenticated == true
                    ? currentTenant.TenantId
                    : Guid.Empty;

            using (LogContext.PushProperty(
                "CorrelationId",
                correlationId))
            using (LogContext.PushProperty(
                "TenantId",
                tenantId == Guid.Empty
                    ? null
                    : tenantId))
            using (LogContext.PushProperty(
                "UserId",
                string.IsNullOrWhiteSpace(userId)
                    ? null
                    : userId))
            {
                try
                {
                    _logger.LogInformation(
                        "Incoming HTTP request {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);

                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();

                    _logger.LogInformation(
                        "Completed HTTP request {Method} {Path} " +
                        "with {StatusCode} in {ElapsedMilliseconds} ms",
                        context.Request.Method,
                        context.Request.Path,
                        context.Response.StatusCode,
                        stopwatch.ElapsedMilliseconds);
                }
            }
        }

        private static string GetOrCreateCorrelationId(
            HttpContext context)
        {
            var incomingCorrelationId =
                context.Request.Headers[
                    CorrelationIdHeader]
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(
                incomingCorrelationId))
            {
                return incomingCorrelationId.Trim();
            }

            return Guid.NewGuid()
                .ToString("N");
        }
    }
}