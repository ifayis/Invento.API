using Dapper;
using Invento.Application.Abstractions;
using Invento.Application.Common;
using Invento.Application.Common.Interface;
using Invento.Application.Features.Suppliers.DTOs;
using Invento.Application.Interfaces;

namespace Invento.Application.Features.Suppliers.Queries
{
    public class GetSupplierLedgerQueryHandler
        : IQueryHandler<
            GetSupplierLedgerQuery,
            ApiResponse<SupplierLedgerDto>>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        private readonly ICurrentTenantService _currentTenant;

        public GetSupplierLedgerQueryHandler(
            IDbConnectionFactory connectionFactory,
            ICurrentTenantService currentTenant)
        {
            _connectionFactory = connectionFactory;
            _currentTenant = currentTenant;
        }

        public async Task<ApiResponse<SupplierLedgerDto>> Handle(
            GetSupplierLedgerQuery request,
            CancellationToken cancellationToken)
        {
            using var connection =
                _connectionFactory.CreateConnection();

            const string supplierSql = @"
            SELECT
                Id AS SupplierId,
                Name AS SupplierName
            FROM Suppliers
            WHERE
                Id=@SupplierId
                AND TenantId=@TenantId
                AND IsDeleted=0;";

            var supplier =
                await connection.QueryFirstOrDefaultAsync<SupplierLedgerDto>(
                    supplierSql,
                    new
                    {
                        request.SupplierId,
                        TenantId = _currentTenant.TenantId
                    });

            if (supplier is null)
            {
                return ApiResponse<SupplierLedgerDto>
                    .FailureResponse(
                        new List<string>
                        {
                            "Supplier not found."
                        });
            }

            const string ledgerSql = @"
            SELECT
                PurchaseDate AS Date,
                'Purchase' AS TransactionType,
                PurchaseNumber AS ReferenceNumber,
                TotalAmount AS Debit,
                CAST(0 AS decimal(18,2)) AS Credit
            FROM Purchases
            WHERE
                SupplierId=@SupplierId
                AND TenantId=@TenantId
                AND IsDeleted=0

            UNION ALL

            SELECT
                PaymentDate AS Date,
                'Payment' AS TransactionType,
                CONCAT('PAY-', CONVERT(varchar(36), Id)) AS ReferenceNumber,
                CAST(0 AS decimal(18,2)) AS Debit,
                Amount AS Credit
            FROM SupplierPayments
            WHERE
                SupplierId=@SupplierId
                AND TenantId=@TenantId
                AND IsDeleted=0

            ORDER BY Date;";

            var transactions =
                (await connection.QueryAsync<SupplierLedgerTransactionDto>(
                    ledgerSql,
                    new
                    {
                        request.SupplierId,
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

            supplier.Transactions = transactions;
            supplier.CurrentOutstanding = balance;

            return ApiResponse<SupplierLedgerDto>
                .SuccessResponse(supplier);
        }
    }
}