using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Commands
{
    public class CreateProductCommand : IRequest<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public string TagNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal CostPrice { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal TaxRate { get; set; }

        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }

    }
}
