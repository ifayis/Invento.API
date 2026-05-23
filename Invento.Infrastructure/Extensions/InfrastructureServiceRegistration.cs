using Invento.Application.Interfaces;
using Invento.Infrastructure.Auth;
using Invento.Infrastructure.Caching;
using Invento.Infrastructure.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Invento.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection
        AddInfrastructureServices(
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

            return services;
        }
    }
}
