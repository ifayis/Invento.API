using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Balance.DTOs;

namespace Invento.Application.Features.Balance.Commands
{
    public class CreateManualIncomeCommand
        : ICommand<ApiResponse<CashTransactionDto>>
    {
        public decimal Amount { get; set; }

        public string Description { get; set; }
            = string.Empty;

        public DateTime TransactionDate { get; set; }
    }
}