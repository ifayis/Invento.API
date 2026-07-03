using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;

namespace Invento.API.Health
{
    public static class HealthCheckResponseWriter
    {
        public static async Task WriteResponseAsync(
            HttpContext context,
            HealthReport report)
        {
            context.Response.ContentType =
                "application/json";

            var response = new
            {
                Status = report.Status.ToString(),

                TotalDuration =
                    report.TotalDuration.TotalMilliseconds,

                Checks = report.Entries.Select(entry => new
                {
                    Name = entry.Key,

                    Status =
                        entry.Value.Status.ToString(),

                    Description =
                        entry.Value.Description,

                    Duration =
                        entry.Value.Duration.TotalMilliseconds
                })
            };

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(
                    response,
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy =
                            JsonNamingPolicy.CamelCase,

                        WriteIndented = true
                    }));
        }
    }
}