using Invento.Application.Common.Interface;
using Invento.Persistence.Data;
using Invento.Persistence.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Invento.Application.Interfaces;
using Invento.Infrastructure.Data;

namespace Invento.Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
                this IServiceCollection services,
                IConfiguration configuration)
            {
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<IApplicationDbContext, AppDbContext>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            return services;
        }
    }
}
