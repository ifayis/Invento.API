namespace Invento.API.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(
            RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers =
                    context.Response.Headers;

                headers["X-Content-Type-Options"] =
                    "nosniff";

                headers["X-Frame-Options"] =
                    "DENY";

                headers["Referrer-Policy"] =
                    "no-referrer";

                headers["Permissions-Policy"] =
                    "camera=(), microphone=(), geolocation=()";

                headers["X-XSS-Protection"] =
                    "0";

                var path =
                    context.Request.Path;

                var isInteractiveUi =
                    path.StartsWithSegments(
                        "/swagger")
                    ||
                    path.StartsWithSegments(
                        "/hangfire");

                if (!isInteractiveUi)
                {
                    headers["Content-Security-Policy"] =
                        "default-src 'none'; " +
                        "base-uri 'none'; " +
                        "form-action 'none'; " +
                        "frame-ancestors 'none'; " +
                        "object-src 'none'";
                }

                headers["Cross-Origin-Opener-Policy"] =
                    "same-origin";

                headers["Cross-Origin-Resource-Policy"] =
                    "same-origin";

                headers["Cross-Origin-Embedder-Policy"] =
                    "require-corp";

                headers.Remove("Server");
                headers.Remove("X-Powered-By");

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}