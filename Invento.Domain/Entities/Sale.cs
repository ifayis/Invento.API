using Invento.Shared.Common;

namespace Invento.Domain.Entities;

public class Sale : BaseEntity
{
    public string InvoiceNumber { get; set; }
        = string.Empty;

    public DateTime SaleDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal ProfitAmount { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<SaleItem> SaleItems
    { get; set; }
        = new List<SaleItem>();
}