using Invento.Application.Behaviours;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    typeof(ApplicationServiceRegistration).Assembly);
            });

            services.AddValidatorsFromAssembly(
                typeof(ApplicationServiceRegistration).Assembly);

            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
