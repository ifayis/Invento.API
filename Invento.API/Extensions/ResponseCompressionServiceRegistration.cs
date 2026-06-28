using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;

namespace Invento.API.Extensions
{
    public static class ResponseCompressionServiceRegistration
    {
        public static IServiceCollection AddResponseCompressionServices(
            this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;

                options.Providers.Add<BrotliCompressionProvider>();

                options.Providers.Add<GzipCompressionProvider>();

                options.MimeTypes =
                    ResponseCompressionDefaults.MimeTypes.Concat(
                        new[]
                        {
                            "application/json",
                            "application/problem+json"
                        });
            });

            services.Configure<BrotliCompressionProviderOptions>(
                options =>
                {
                    options.Level =
                        CompressionLevel.Fastest;
                });

            services.Configure<GzipCompressionProviderOptions>(
                options =>
                {
                    options.Level =
                        CompressionLevel.Fastest;
                });

            return services;
        }
    }
}