using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.DTOs
{
    public class SaleDetailsDto
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }
            = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal SubTotal { get; set; }

        public decimal TaxAmount { get; set; }

        public decimal DiscountAmount { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ProfitAmount { get; set; }

        public List<SaleItemDto> Items
        { get; set; }
            = new();
    }
}
