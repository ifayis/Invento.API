using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Features.Payables.DTOs;

namespace Invento.Application.Features.Payables.Commands
{
    public class RecordSupplierPaymentCommand
        : ICommand<ApiResponse<SupplierPaymentDto>>
    {
        public Guid PurchaseId { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string Remarks { get; set; }
            = string.Empty;
    }
}