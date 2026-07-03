using Hangfire;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Invento.API.Health
{
    public class HangfireHealthCheck
        : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var monitoringApi =
                    JobStorage.Current.GetMonitoringApi();

                var servers =
                    monitoringApi.Servers();

                if (servers is null ||
                    servers.Count == 0)
                {
                    return Task.FromResult(
                        HealthCheckResult.Unhealthy(
                            "No active Hangfire servers found."));
                }

                return Task.FromResult(
                    HealthCheckResult.Healthy(
                        $"Hangfire is running with {servers.Count} active server(s)."));
            }
            catch (Exception ex)
            {
                return Task.FromResult(
                    HealthCheckResult.Unhealthy(
                        "Hangfire storage is unavailable.",
                        ex));
            }
        }
    }
}