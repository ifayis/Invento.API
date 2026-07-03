using Invento.API.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Invento.API.Extensions
{
    public static class HealthCheckServiceRegistration
    {
        public static IServiceCollection AddHealthCheckServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var sqlConnectionString =
                configuration.GetConnectionString(
                    "DefaultConnection");

            var redisConnectionString =
                configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(
                sqlConnectionString))
            {
                throw new InvalidOperationException(
                    "DefaultConnection is not configured.");
            }

            if (string.IsNullOrWhiteSpace(
                redisConnectionString))
            {
                throw new InvalidOperationException(
                    "Redis:ConnectionString is not configured.");
            }

            services
                .AddHealthChecks()

                .AddCheck(
                    "api",
                    () => HealthCheckResult.Healthy(
                        "Invento API is running."),
                    tags: new[] { "live" })

                .AddSqlServer(
                    connectionString:
                        sqlConnectionString,
                    name: "sql-server",
                    failureStatus:
                        HealthStatus.Unhealthy,
                    tags: new[]
                    {
                        "ready",
                        "database"
                    })

                .AddRedis(
                    redisConnectionString,
                    name: "redis",
                    failureStatus:
                        HealthStatus.Unhealthy,
                    tags: new[]
                    {
                        "ready",
                        "cache"
                    })

                .AddCheck<HangfireHealthCheck>(
                    name: "hangfire",
                    failureStatus:
                        HealthStatus.Unhealthy,
                    tags: new[]
                    {
                        "ready",
                        "background-jobs"
                    });

            return services;
        }
    }
}