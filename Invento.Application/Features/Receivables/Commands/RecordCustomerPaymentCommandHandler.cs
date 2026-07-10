using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Services;
using Invento.Application.Features.Receivables.DTOs;
using Invento.Application.Interfaces;
using Invento.Domain.Entities;
using Invento.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Invento.Application.Features.Receivables.Commands
{
    public class RecordCustomerPaymentCommandHandler
        : ICommandHandler<
            RecordCustomerPaymentCommand,
            ApiResponse<CustomerPaymentDto>>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentTenantService _currentTenant;
        private readonly CashTransactionService _cashTransactionService;

        public RecordCustomerPaymentCommandHandler(
            IApplicationDbContext context,
            ICurrentTenantService currentTenant,
            CashTransactionService cashTransactionService)
        {
            _context = context;
            _currentTenant = currentTenant;
            _cashTransactionService = cashTransactionService;
        }

        public async Task<ApiResponse<CustomerPaymentDto>> Handle(
            RecordCustomerPaymentCommand request,
            CancellationToken cancellationToken)
        {
            var tenantId =
                _currentTenant.TenantId;

            await using var transaction =
                await _context.BeginTransactionAsync(
                    cancellationToken);

            try
            {
                var sale =
                    await _context.Sales
                        .FirstOrDefaultAsync(
                            x =>
                                x.Id == request.SaleId
                                && x.TenantId == tenantId
                                && !x.IsDeleted,
                            cancellationToken);

                if (sale is null)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Sale not found"
                            });
                }

                if (!sale.CustomerId.HasValue)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Sale has no customer"
                            });
                }

                if (request.Amount <= 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Payment amount must be greater than zero"
                            });
                }

                if (sale.DueAmount <= 0)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Sale has no outstanding amount"
                            });
                }

                if (request.Amount > sale.DueAmount)
                {
                    await transaction.RollbackAsync(
                        cancellationToken);

                    _context.ClearChangeTracker();

                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new List<string>
                            {
                                "Payment amount exceeds due amount"
                            });
                }

                var payment =
                    new CustomerPayment
                    {
                        TenantId = tenantId,
                        SaleId = sale.Id,
                        CustomerId = sale.CustomerId.Value,
                        Amount = request.Amount,
                        PaymentDate = request.PaymentDate,
                        Remarks = request.Remarks
                            ?? string.Empty
                    };

                await _context.CustomerPayments
                    .AddAsync(
                        payment,
                        cancellationToken);

                sale.PaidAmount +=
                    request.Amount;

                sale.DueAmount =
                    sale.TotalAmount
                    - sale.PaidAmount;

                if (sale.DueAmount < 0)
                {
                    sale.DueAmount = 0;
                }

                sale.PaymentStatus =
                    sale.DueAmount == 0
                        ? PaymentStatus.Paid
                        : sale.PaidAmount == 0
                            ? PaymentStatus.Unpaid
                            : PaymentStatus.PartiallyPaid;

                await _cashTransactionService
                    .CreateTransaction(
                        CashTransactionType.Sale,
                        request.Amount,
                        $"Customer Payment - {sale.InvoiceNumber}",
                        request.PaymentDate,
                        cancellationToken);

                await _context.SaveChangesAsync(
                    cancellationToken);

                await transaction.CommitAsync(
                    cancellationToken);

                return ApiResponse<CustomerPaymentDto>
                    .SuccessResponse(
                        new CustomerPaymentDto
                        {
                            Id = payment.Id,
                            SaleId = payment.SaleId,
                            CustomerId = payment.CustomerId,
                            Amount = payment.Amount,
                            PaymentDate = payment.PaymentDate,
                            Remarks = payment.Remarks
                        },
                        "Payment recorded successfully");
            }
            catch
            {
                await transaction.RollbackAsync(
                    CancellationToken.None);

                _context.ClearChangeTracker();

                throw;
            }
        }
    }
}