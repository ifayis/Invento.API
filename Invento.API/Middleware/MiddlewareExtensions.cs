namespace Invento.API.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder
            UseCustomExceptionMiddleware(
                this IApplicationBuilder app)
        {
            return app.UseMiddleware<
                ExceptionHandlingMiddleware>();
        }

        public static IApplicationBuilder
            UseRequestLoggingMiddleware(
                this IApplicationBuilder app)
        {
            return app.UseMiddleware<
                RequestLoggingMiddleware>();
        }
    }
}