using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Products.DTOs;

namespace Invento.Application.Features.Products.Commands
{
    public class UpdateProductCommand
        : ICommand<ApiResponse<ProductDto>>
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

        public decimal SellingPrice { get; set; }

        public Guid CategoryId { get; set; }

        public int LowStockThreshold { get; set; }

        public int CriticalStockThreshold { get; set; } 
    }
}
