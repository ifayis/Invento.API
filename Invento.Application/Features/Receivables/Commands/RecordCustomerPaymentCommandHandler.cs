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
            using var transaction =
                await _context.BeginTransactionAsync();

            try
            {
                var sale = await _context.Sales
                    .FirstOrDefaultAsync(
                        x =>
                            x.Id == request.SaleId
                            && x.TenantId ==
                                _currentTenant.TenantId
                            && !x.IsDeleted,
                        cancellationToken);

                if (sale is null)
                {
                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new()
                            {
                                "Sale not found"
                            });
                }

                if (!sale.CustomerId.HasValue)
                {
                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new()
                            {
                                "Sale has no customer"
                            });
                }

                if (request.Amount <= 0)
                {
                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new()
                            {
                                "Payment amount must be greater than zero"
                            });
                }

                if (request.Amount > sale.DueAmount)
                {
                    return ApiResponse<CustomerPaymentDto>
                        .FailureResponse(
                            new()
                            {
                                "Payment amount exceeds due amount"
                            });
                }

                var payment = new CustomerPayment
                {
                    TenantId = _currentTenant.TenantId,

                    SaleId = sale.Id,

                    CustomerId = sale.CustomerId.Value,

                    Amount = request.Amount,

                    PaymentDate = request.PaymentDate,

                    Remarks = request.Remarks
                };

                await _context.CustomerPayments
                    .AddAsync(
                        payment,
                        cancellationToken);

                sale.PaidAmount += request.Amount;

                sale.DueAmount -= request.Amount;

                sale.PaymentStatus =
                    sale.DueAmount == 0
                        ? PaymentStatus.Paid
                        : PaymentStatus.PartiallyPaid;

                await _cashTransactionService
                    .CreateTransaction(
                        CashTransactionType.Sale,
                        request.Amount,
                        $"Customer Payment - {sale.InvoiceNumber}",
                        request.PaymentDate);

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
                    cancellationToken);

                throw;
            }
        }
    }
}