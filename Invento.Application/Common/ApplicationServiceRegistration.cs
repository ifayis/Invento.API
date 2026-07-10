using FluentValidation;
using Invento.Application.Behaviours;
using Invento.Application.Common.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Invento.Application.Common
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection
            AddApplicationServices(
                this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(
                    typeof(
                        ApplicationServiceRegistration)
                    .Assembly);
            });

            services.AddValidatorsFromAssembly(
                typeof(
                    ApplicationServiceRegistration)
                .Assembly);

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(CachingBehavior<,>));

            services.AddScoped<
                StockMovementService>();

            services.AddScoped<
                CashTransactionService>();

            return services;
        }
    }
}