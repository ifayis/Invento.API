using Invento.Application.Common.Jobs;
using Invento.Application.Interfaces;
using Invento.Infrastructure.Auth;
using Invento.Infrastructure.Caching;
using Invento.Infrastructure.Jobs;
using Invento.Infrastructure.Tenancy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hangfire;
using Hangfire.SqlServer;

namespace Invento.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            services.AddScoped<ICurrentTenantService, CurrentTenantService>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    configuration["Redis:ConnectionString"];
            });

            services.AddScoped<ICacheService, RedisCacheService>();

            services.AddScoped<
                IRecurringJobService,
                RecurringJobService>();

            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(
                        CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(
                        configuration.GetConnectionString("DefaultConnection"),
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout =
                                TimeSpan.FromMinutes(5),

                            SlidingInvisibilityTimeout =
                                TimeSpan.FromMinutes(5),

                            QueuePollInterval =
                                TimeSpan.FromSeconds(15),

                            UseRecommendedIsolationLevel = true,

                            DisableGlobalLocks = true
                        });
            });

            services.AddHangfireServer();

            return services;
        }
    }
}
