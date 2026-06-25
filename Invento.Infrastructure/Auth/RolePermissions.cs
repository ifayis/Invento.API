using Invento.Application.Common;
using Invento.Domain.Enums;

namespace Invento.Infrastructure.Auth
{
    public static class RolePermissions
    {
        public static IReadOnlyList<string>
            GetPermissions(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => new[]
                {
                    Permissions.Dashboard,
                    Permissions.Categories,
                    Permissions.Products,
                    Permissions.Customers,
                    Permissions.Suppliers,
                    Permissions.Sales,
                    Permissions.Purchases,
                    Permissions.StockMovements,
                    Permissions.Reports,
                    Permissions.Receivables,
                    Permissions.Payables,
                    Permissions.Balance,
                    Permissions.Profit,
                    Permissions.Targets,
                    Permissions.Company,
                    Permissions.Users
                },

                UserRole.Manager => new[]
                {
                    Permissions.Dashboard,
                    Permissions.Categories,
                    Permissions.Products,
                    Permissions.Customers,
                    Permissions.Suppliers,
                    Permissions.Sales,
                    Permissions.Purchases,
                    Permissions.StockMovements,
                    Permissions.Reports,
                    Permissions.Receivables,
                    Permissions.Payables,
                    Permissions.Balance,
                    Permissions.Profit
                },

                UserRole.Staff => new[]
                {
                    Permissions.Dashboard,
                    Permissions.Customers,
                    Permissions.Sales
                },

                _ => Array.Empty<string>()
            };
        }
    }
}