using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Balance.DTOs;
using Invento.Shared.Pagination;

namespace Invento.Application.Features.Balance.Queries
{
    public class GetCashTransactionsQuery
        : IQuery<ApiResponse<PagedResponse<CashTransactionDto>>>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}