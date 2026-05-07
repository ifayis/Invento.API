using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Products.Queries
{
    public class GetProductsQuery : IRequest<IEnumerable<ProductDto>> { }

    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public decimal SellingPrice { get; set; }
        public int StockQuantity { get; set; }
    }
}
