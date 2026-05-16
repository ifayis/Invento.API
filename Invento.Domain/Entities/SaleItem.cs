using Invento.Shared.Common;

namespace Invento.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid TenantId { get; set; }

    public Guid SaleId { get; set; }

    public Sale Sale { get; set; } = null!;

    public Guid ProductId { get; set; }

    public Product Product { get; set; } = null!;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal CostPrice { get; set; }

    public decimal TaxRate { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal ProfitAmount { get; set; }
}