using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ImgaeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_TenantId_PurchaseId_IsDeleted",
                table: "SupplierPayments",
                columns: new[] { "TenantId", "PurchaseId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_SupplierPayments_TenantId_SupplierId_IsDeleted",
                table: "SupplierPayments",
                columns: new[] { "TenantId", "SupplierId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_DueAmount_IsDeleted",
                table: "Sales",
                columns: new[] { "TenantId", "DueAmount", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_DueAmount_IsDeleted",
                table: "Purchases",
                columns: new[] { "TenantId", "DueAmount", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_CategoryId_IsDeleted",
                table: "Products",
                columns: new[] { "TenantId", "CategoryId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_TenantId_CustomerId_IsDeleted",
                table: "CustomerPayments",
                columns: new[] { "TenantId", "CustomerId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPayments_TenantId_SaleId_IsDeleted",
                table: "CustomerPayments",
                columns: new[] { "TenantId", "SaleId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_TenantId_TransactionType_IsDeleted",
                table: "CashTransactions",
                columns: new[] { "TenantId", "TransactionType", "IsDeleted" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupplierPayments_TenantId_PurchaseId_IsDeleted",
                table: "SupplierPayments");

            migrationBuilder.DropIndex(
                name: "IX_SupplierPayments_TenantId_SupplierId_IsDeleted",
                table: "SupplierPayments");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_DueAmount_IsDeleted",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_DueAmount_IsDeleted",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_CategoryId_IsDeleted",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPayments_TenantId_CustomerId_IsDeleted",
                table: "CustomerPayments");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPayments_TenantId_SaleId_IsDeleted",
                table: "CustomerPayments");

            migrationBuilder.DropIndex(
                name: "IX_CashTransactions_TenantId_TransactionType_IsDeleted",
                table: "CashTransactions");
        }
    }
}
