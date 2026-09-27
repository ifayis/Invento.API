using System;

namespace Invento.Application.Features.Sales.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }

        public Guid? CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string InvoiceNumber { get; set; } = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ProfitAmount { get; set; }

        public bool IsDeleted { get; set; }
    }
}