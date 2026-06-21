using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Payables.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Payables.Commands
{
    public class RecordSupplierPaymentCommandHandler
        : ICommandHandler<
            RecordSupplierPaymentCommand,
            ApiResponse<SupplierPaymentDto>>
    {
        private readonly IApplicationDbContext _context;

        private readonly ICurrentTenantService _currentTenant;

        private readonly CashTransactionService _cashTransactionService;

        public RecordSupplierPaymentCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            CashTransactionService cashTransactionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cashTransactionService = cashTransactionService;
        }

        public async Task<ApiResponse<SupplierPaymentDto>> Handle(
            RecordSupplierPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var purchase = await _context.Purchases
                .Include(x => x.Supplier)
                .FirstOrDefaultAsync(
                    x =>
                        x.Id == request.PurchaseId
                        && x.TenantId == _currentTenant.TenantId
                        && !x.IsDeleted,
                    cancellationToken);

            if (purchase is null)
            {
                return ApiResponse<SupplierPaymentDto>
                    .FailureResponse(
                        ["Purchase not found"]);
            }

            if (request.Amount <= 0)
            {
                return ApiResponse<SupplierPaymentDto>
                    .FailureResponse(
                        ["Invalid payment amount"]);
            }

            if (request.Amount > purchase.DueAmount)
            {
                return ApiResponse<SupplierPaymentDto>
                    .FailureResponse(
                        ["Payment exceeds outstanding amount"]);
            }

            var payment = new SupplierPayment
            {
                TenantId = _currentTenant.TenantId,

                PurchaseId = purchase.Id,

                SupplierId = purchase.SupplierId,

                Amount = request.Amount,

                PaymentDate = request.PaymentDate,

                Remarks = request.Remarks
            };

            purchase.PaidAmount += request.Amount;

            purchase.DueAmount =
                purchase.TotalAmount - purchase.PaidAmount;

            purchase.PaymentStatus =
                purchase.DueAmount <= 0
                    ? PaymentStatus.Paid
                    : PaymentStatus.PartiallyPaid;

            await _context.SupplierPayments
                .AddAsync(
                    payment,
                    cancellationToken);

            await _cashTransactionService
                .CreateTransaction(
                    CashTransactionType.Purchase,
                    request.Amount,
                    $"Supplier Payment - {purchase.PurchaseNumber}",
                    request.PaymentDate);

            await _context.SaveChangesAsync(
                cancellationToken);

            return ApiResponse<SupplierPaymentDto>
                .SuccessResponse(
                    new SupplierPaymentDto
                    {
                        Id = payment.Id,

                        PurchaseId = payment.PurchaseId,

                        SupplierId = payment.SupplierId,

                        Amount = payment.Amount,

                        PaymentDate = payment.PaymentDate,

                        Remarks = payment.Remarks
                    },
                    "Supplier payment recorded successfully");
        }
    }
}