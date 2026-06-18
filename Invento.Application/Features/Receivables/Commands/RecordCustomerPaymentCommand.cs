using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Receivables.DTOs;

namespace Invento.Application.Features.Receivables.Commands
{
    public class RecordCustomerPaymentCommand
        : ICommand<ApiResponse<CustomerPaymentDto>>
    {
        public Guid SaleId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}