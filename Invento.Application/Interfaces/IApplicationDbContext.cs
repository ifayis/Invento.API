using Invento.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }

        DbSet<Tenant> Tenants { get; }

        DbSet<RefreshToken> RefreshTokens { get; }

        DbSet<Product> Products { get; }

        DbSet<Category> Categories { get; }

        DbSet<StockMovement> StockMovements { get; }

        DbSet<Sale> Sales { get; }

        DbSet<SaleItem> SaleItems { get; }

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken);

        Task<IDbContextTransaction>
            BeginTransactionAsync();
    }
}
