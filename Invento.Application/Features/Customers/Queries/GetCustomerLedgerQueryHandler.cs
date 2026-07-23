using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Customers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Customers.Queries
{
    public class GetCustomerLedgerQueryHandler
        : IQueryHandler<
            GetCustomerLedgerQuery,
            ApiResponse<CustomerLedgerDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ICurrentTenantService _currentTenant;

        public GetCustomerLedgerQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<CustomerLedgerDto>> Handle(
            GetCustomerLedgerQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string customerSql = @"
            SELECT
                Id AS CustomerId,
                Name AS CustomerName
            FROM Customers
            WHERE
                Id=@CustomerId
                AND TenantId=@TenantId
                AND IsDeleted=0;";

            var customer =
                await connection.QueryFirstOrDefaultAsync<CustomerLedgerDto>(
                    customerSql,
                    new
                    {
                        request.CustomerId,
                        TenantId = _currentTenant.TenantId
                    });

            if (customer is null)
            {
                return ApiResponse<CustomerLedgerDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Customer not found."
                        });
            }

            const string ledgerSql = @"
            SELECT
                SaleDate AS Date,
                'Sale' AS TransactionType,
                InvoiceNumber AS ReferenceNumber,
                TotalAmount AS Debit,
                CAST(0 AS decimal(18,2)) AS Credit
            FROM Sales
            WHERE
                CustomerId=@CustomerId
                AND TenantId=@TenantId
                AND IsDeleted=0

            UNION ALL

            SELECT
                PaymentDate AS Date,
                'Payment' AS TransactionType,
                CONCAT('PAY-', CONVERT(varchar(36), Id)) AS ReferenceNumber,
                CAST(0 AS decimal(18,2)) AS Debit,
                Amount AS Credit
            FROM CustomerPayments
            WHERE
                CustomerId=@CustomerId
                AND TenantId=@TenantId
                AND IsDeleted=0
            ORDER BY Date;";

            var transactions =
                (await connection.QueryAsync<CustomerLedgerTransactionDto>(
                    ledgerSql,
                    new
                    {
                        request.CustomerId,
                        TenantId = _currentTenant.TenantId
                    }))
                .ToList();

            decimal balance = 0;

            foreach (var transaction in transactions)
            {
                balance += transaction.Debit;
                balance -= transaction.Credit;

                transaction.RunningBalance = balance;
            }

            customer.Transactions = transactions;
            customer.CurrentOutstanding = balance;

            return ApiResponse<CustomerLedgerDto>
                .SuccessResponse(customer);
        }
    }
}