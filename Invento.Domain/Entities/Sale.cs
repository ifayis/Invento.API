using Invento.Domain.Enums;
using Invento.Shared.Common;

namespace Invento.Domain.Entities;

public class Sale : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public Guid? CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public Guid TenantId { get; set; }

    public DateTime SaleDate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal PaidAmount { get; set; }

    public decimal DueAmount { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public decimal ProfitAmount { get; set; }

    public bool IsDeleted { get; set; } = false;

    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();

    public ICollection<CustomerPayment> Payments { get; set; } = new List<CustomerPayment>();
}