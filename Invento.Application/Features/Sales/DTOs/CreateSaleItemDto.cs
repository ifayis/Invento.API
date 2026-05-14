using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.DTOs
{
    public class CreateSaleItemDto
    {
        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
