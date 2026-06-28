using Asp.Versioning;
using Asp.Versioning.ApiExplorer;

namespace Invento.API.Extensions
{
    public static class ApiVersioningServiceRegistration
    {
        public static IServiceCollection AddApiVersioningServices(
            this IServiceCollection services)
        {
            services
                .AddApiVersioning(options =>
                {
                    options.DefaultApiVersion =
                        new ApiVersion(1, 0);

                    options.AssumeDefaultVersionWhenUnspecified =
                        true;

                    options.ReportApiVersions = true;

                    options.ApiVersionReader =
                        new UrlSegmentApiVersionReader();
                })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";

                    options.SubstituteApiVersionInUrl = true;
                });

            return services;
        }
    }
}