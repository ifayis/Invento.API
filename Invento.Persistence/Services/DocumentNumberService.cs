using System.Data;
using Invento.Application.Interfaces;
using Invento.Persistence.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Invento.Persistence.Services
{
    public class DocumentNumberService
        : IDocumentNumberService
    {
        private readonly AppDbContext _context;

        public DocumentNumberService(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateNextAsync(
            Guid tenantId,
            string documentType,
            string prefix,
            DateTime documentDate,
            CancellationToken cancellationToken = default)
        {
            if (tenantId == Guid.Empty)
            {
                throw new ArgumentException(
                    "Tenant id is required.",
                    nameof(tenantId));
            }

            if (string.IsNullOrWhiteSpace(documentType))
            {
                throw new ArgumentException(
                    "Document type is required.",
                    nameof(documentType));
            }

            if (string.IsNullOrWhiteSpace(prefix))
            {
                throw new ArgumentException(
                    "Document prefix is required.",
                    nameof(prefix));
            }

            var currentTransaction =
                _context.Database.CurrentTransaction;

            if (currentTransaction is null)
            {
                throw new InvalidOperationException(
                    "Document number generation requires " +
                    "an active database transaction.");
            }

            var normalizedDocumentType =
                documentType
                    .Trim()
                    .ToUpperInvariant();

            var normalizedPrefix =
                prefix
                    .Trim()
                    .ToUpperInvariant();

            var periodKey =
                documentDate.ToUniversalTime()
                    .ToString("yyyyMM");

            var lockResource =
                $"Invento:DocumentNumber:" +
                $"{tenantId:N}:" +
                $"{normalizedDocumentType}:" +
                $"{periodKey}";

            var connection =
                _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await _context.Database
                    .OpenConnectionAsync(
                        cancellationToken);
            }

            await using var command =
                connection.CreateCommand();

            command.Transaction =
                currentTransaction.GetDbTransaction();

            command.CommandText = """
                DECLARE @LockResult int;

                EXEC @LockResult = sys.sp_getapplock
                    @Resource = @LockResource,
                    @LockMode = 'Exclusive',
                    @LockOwner = 'Transaction',
                    @LockTimeout = 10000;

                IF @LockResult < 0
                BEGIN
                    THROW 51000,
                        'Could not acquire document number lock.',
                        1;
                END;

                DECLARE @IssuedNumber bigint;

                SELECT
                    @IssuedNumber = NextNumber
                FROM DocumentNumberSequences
                WHERE
                    TenantId = @TenantId
                    AND DocumentType = @DocumentType
                    AND PeriodKey = @PeriodKey;

                IF @IssuedNumber IS NULL
                BEGIN
                    SET @IssuedNumber = 1;

                    INSERT INTO DocumentNumberSequences
                    (
                        Id,
                        TenantId,
                        DocumentType,
                        PeriodKey,
                        NextNumber
                    )
                    VALUES
                    (
                        NEWID(),
                        @TenantId,
                        @DocumentType,
                        @PeriodKey,
                        2
                    );
                END
                ELSE
                BEGIN
                    UPDATE DocumentNumberSequences
                    SET NextNumber = NextNumber + 1
                    WHERE
                        TenantId = @TenantId
                        AND DocumentType = @DocumentType
                        AND PeriodKey = @PeriodKey;
                END;

                SELECT @IssuedNumber;
                """;

            AddParameter(
                command,
                "@LockResource",
                SqlDbType.NVarChar,
                lockResource,
                255);

            AddParameter(
                command,
                "@TenantId",
                SqlDbType.UniqueIdentifier,
                tenantId);

            AddParameter(
                command,
                "@DocumentType",
                SqlDbType.NVarChar,
                normalizedDocumentType,
                50);

            AddParameter(
                command,
                "@PeriodKey",
                SqlDbType.NVarChar,
                periodKey,
                20);

            var result =
                await command.ExecuteScalarAsync(
                    cancellationToken);

            if (result is null
                || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Document number generation " +
                    "did not return a sequence value.");
            }

            var issuedNumber =
                Convert.ToInt64(result);

            return
                $"{normalizedPrefix}-" +
                $"{periodKey}-" +
                $"{issuedNumber:D4}";
        }

        private static void AddParameter(
            System.Data.Common.DbCommand command,
            string name,
            SqlDbType sqlDbType,
            object value,
            int? size = null)
        {
            var parameter =
                new SqlParameter(
                    name,
                    sqlDbType)
                {
                    Value = value
                };

            if (size.HasValue)
            {
                parameter.Size =
                    size.Value;
            }

            command.Parameters.Add(
                parameter);
        }
    }
}