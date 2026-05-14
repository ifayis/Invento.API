using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.DTOs
{
    public class SaleDto
    {
        public Guid Id { get; set; }

        public string InvoiceNumber { get; set; }
            = string.Empty;

        public DateTime SaleDate { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ProfitAmount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
