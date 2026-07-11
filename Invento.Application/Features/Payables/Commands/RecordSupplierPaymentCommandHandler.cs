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
            var tenantId =
                _currentTenant.TenantId;

            var strategy =
                _context.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(
                async () =>
                {

                    await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);

                    try
                    {
                        var purchase =
                            await _context.Purchases
                                .FirstOrDefaultAsync(
                                    x =>
                                        x.Id == request.PurchaseId
                                        && x.TenantId == tenantId
                                        && !x.IsDeleted,
                                    cancellationToken);

                        if (purchase is null)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<SupplierPaymentDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Purchase not found"
                                    });
                        }

                        if (request.Amount <= 0)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<SupplierPaymentDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Payment amount must be greater than zero"
                                    });
                        }

                        if (purchase.DueAmount <= 0)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<SupplierPaymentDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Purchase has no outstanding amount"
                                    });
                        }

                        if (request.Amount > purchase.DueAmount)
                        {
                            await transaction.RollbackAsync(
                                cancellationToken);

                            _context.ClearChangeTracker();

                            return ApiResponse<SupplierPaymentDto>
                                .FailureResponse(
                                    new List<string>
                                    {
                                "Payment amount exceeds due amount"
                                    });
                        }

                        var payment =
                            new SupplierPayment
                            {
                                TenantId = tenantId,
                                PurchaseId = purchase.Id,
                                SupplierId = purchase.SupplierId,
                                Amount = request.Amount,
                                PaymentDate = request.PaymentDate,
                                Remarks = request.Remarks
                                    ?? string.Empty
                            };

                        await _context.SupplierPayments
                            .AddAsync(
                                payment,
                                cancellationToken);

                        purchase.PaidAmount +=
                            request.Amount;

                        purchase.DueAmount =
                            purchase.TotalAmount
                            - purchase.PaidAmount;

                        if (purchase.DueAmount < 0)
                        {
                            purchase.DueAmount = 0;
                        }

                        purchase.PaymentStatus =
                            purchase.DueAmount == 0
                                ? PaymentStatus.Paid
                                : purchase.PaidAmount == 0
                                    ? PaymentStatus.Unpaid
                                    : PaymentStatus.PartiallyPaid;

                        await _cashTransactionService
                            .CreateTransaction(
                                CashTransactionType.Purchase,
                                request.Amount,
                                $"Supplier Payment - {purchase.PurchaseNumber}",
                                request.PaymentDate,
                                cancellationToken);

                        await _context.SaveChangesAsync(
                            cancellationToken);

                        await transaction.CommitAsync(
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
                    catch
                    {
                        await transaction.RollbackAsync(
                            CancellationToken.None);

                        _context.ClearChangeTracker();

                        throw;
                    }
                }
            );
        }
    }
}