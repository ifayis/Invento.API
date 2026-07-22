using Hangfire;
using Hangfire.SqlServer;
using Invento.Application.Common;
using Invento.Application.Common.Jobs;
using Invento.Application.Interfaces;
using Invento.Infrastructure.Auth;
using Invento.Infrastructure.Caching;
using Invento.Infrastructure.Email;
using Invento.Infrastructure.Jobs;
using Invento.Infrastructure.Storage;
using Invento.Infrastructure.Tenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Invento.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<
                IJwtTokenGenerator, 
                JwtTokenGenerator>();

            services.AddScoped<
                ICurrentTenantService, 
                CurrentTenantService>();

            var redisConnectionString =
                configuration["Redis:ConnectionString"];

            if (string.IsNullOrWhiteSpace(
                redisConnectionString))
            {
                throw new InvalidOperationException(
                    "Redis:ConnectionString is not configured.");
            }

            services.AddSingleton<IConnectionMultiplexer>(_ =>
            {
                return ConnectionMultiplexer.Connect(
                    redisConnectionString);
            });


            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    redisConnectionString;

                options.InstanceName =
                    configuration["Redis:InstanceName"]
                    ?? "Invento:";
            });

            services.AddScoped<
                ICacheService, 
                RedisCacheService>();

            services.AddSingleton<
                ICacheVersionService,
                RedisCacheVersionService>();

            services.AddScoped<
                IRecurringJobService,
                RecurringJobService>();

            services.AddScoped<
                ICurrentUserService,
                CurrentUserService>();

            services.AddScoped<
                IFileStorageService,
                LocalFileStorageService>();

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

            services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));

            services.AddScoped<
                IEmailService,
                SmtpEmailService>();

            var runHangfireServer =
                configuration.GetValue<bool>(
                    "Hangfire:RunServer");

            if (runHangfireServer)
            {
                services.AddHangfireServer();
            }

            return services;
        }
    }
}
