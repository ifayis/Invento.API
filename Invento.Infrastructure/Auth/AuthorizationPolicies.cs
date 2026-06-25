using Invento.Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Invento.Infrastructure.Auth
{
    public static class AuthorizationPolicies
    {
        public static IServiceCollection AddPermissionPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    Permissions.Dashboard,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Dashboard)));

                options.AddPolicy(
                    Permissions.Products,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Products)));

                options.AddPolicy(
                    Permissions.Categories,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Categories)));

                options.AddPolicy(
                    Permissions.Customers,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Customers)));

                options.AddPolicy(
                    Permissions.Suppliers,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Suppliers)));

                options.AddPolicy(
                    Permissions.Sales,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Sales)));

                options.AddPolicy(
                    Permissions.Purchases,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Purchases)));

                options.AddPolicy(
                    Permissions.StockMovements,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.StockMovements)));

                options.AddPolicy(
                    Permissions.Reports,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Reports)));

                options.AddPolicy(
                    Permissions.Receivables,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Receivables)));

                options.AddPolicy(
                    Permissions.Payables,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Payables)));

                options.AddPolicy(
                    Permissions.Balance,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Balance)));

                options.AddPolicy(
                    Permissions.Profit,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Profit)));

                options.AddPolicy(
                    Permissions.Targets,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Targets)));

                options.AddPolicy(
                    Permissions.Company,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Company)));

                options.AddPolicy(
                    Permissions.Users,
                    policy => policy.Requirements.Add(
                        new PermissionRequirement(
                            Permissions.Users)));
            });

            return services;
        }
    }
}