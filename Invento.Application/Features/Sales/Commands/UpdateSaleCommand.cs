using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Sales.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invento.Application.Features.Sales.Command
{
    public class UpdateSaleCommand
        : ICommand<ApiResponse<Guid>>
    {
        public Guid Id { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal DiscountAmount { get; set; }

        public List<CreateSaleItemDto> Items
        { get; set; }
            = new();
    }
}
