using Invento.Application.Interfaces;
using Invento.Infrastructure.Auth;
using Invento.Infrastructure.Caching;
using Invento.Infrastructure.Tenancy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Infrastructure.Extensions
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IJwtTokenGenerator,
                JwtTokenGenerator>();

            services.AddScoped<ICurrentTenantService,
                CurrentTenantService>();

            services.AddStackExchangeRedisCache(options =>
            {
                var serviceProvider = services.BuildServiceProvider();
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                options.Configuration = configuration["Redis:ConnectionString"];
            });

            services.AddScoped<
                ICacheService,
                RedisCacheService>();

            return services;
        }

        
    }
}
