using Invento.Application.Common.Interface;
using Invento.persistance.Data;
using Invento.Persistence.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Invento.Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection
            AddPersistenceServices(
                this IServiceCollection services,
                IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            return services;
        }
    }
}
