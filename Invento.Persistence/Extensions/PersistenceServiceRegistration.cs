using Invento.Application.Common.Interface;
using Invento.Application.Interfaces;
using Invento.Persistence.Connections;
using Invento.Persistence.Data;
using Invento.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Invento.Persistence.Extensions
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString(
                    "DefaultConnection");

            if (string.IsNullOrWhiteSpace(
                connectionString))
            {
                throw new InvalidOperationException(
                    "ConnectionStrings:DefaultConnection " +
                    "is not configured.");
            }

            services.AddScoped<
                IDbConnectionFactory,
                SqlConnectionFactory>();

            services.AddScoped<
                IApplicationDbContext,
                AppDbContext>();

            services.AddScoped<
                IDocumentNumberService,
                DocumentNumberService>();

            services.AddDbContext<AppDbContext>(
                options =>
                {
                    options.UseSqlServer(
                        connectionString,
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                                maxRetryCount: 3,
                                maxRetryDelay:
                                    TimeSpan.FromSeconds(5),
                                errorNumbersToAdd: null);

                            sqlOptions.CommandTimeout(30);
                        });

                    options.EnableDetailedErrors(false);

                    options.EnableSensitiveDataLogging(false);
                });

            return services;
        }
    }
}