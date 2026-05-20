using Invento.Application.Abstractions;
using Invento.Application.Common;

namespace Invento.Application.Features.Profit.Queries
{
    public class GetProductProfitQuery
        : IQuery<ApiResponse<decimal>>
    {
        public Guid ProductId { get; set; }
    }
}
