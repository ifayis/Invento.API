using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_PurchaseNumber",
                table: "Purchases");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_IsDeleted",
                table: "StockMovements",
                columns: new[] { "TenantId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_MovementType_IsDeleted",
                table: "StockMovements",
                columns: new[] { "TenantId", "MovementType", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_ProductId_IsDeleted",
                table: "StockMovements",
                columns: new[] { "TenantId", "ProductId", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_CustomerId_IsDeleted_SaleDate",
                table: "Sales",
                columns: new[] { "TenantId", "CustomerId", "IsDeleted", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales",
                columns: new[] { "TenantId", "InvoiceNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_IsDeleted_SaleDate",
                table: "Sales",
                columns: new[] { "TenantId", "IsDeleted", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate",
                table: "Purchases",
                columns: new[] { "TenantId", "IsDeleted", "PurchaseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_PurchaseNumber",
                table: "Purchases",
                columns: new[] { "TenantId", "PurchaseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate",
                table: "Purchases",
                columns: new[] { "TenantId", "SupplierId", "IsDeleted", "PurchaseDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_IsDeleted",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_MovementType_IsDeleted",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_ProductId_IsDeleted",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_CustomerId_IsDeleted_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_InvoiceNumber",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_IsDeleted_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_PurchaseNumber",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_InvoiceNumber",
                table: "Sales",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleDate",
                table: "Sales",
                column: "SaleDate");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseDate",
                table: "Purchases",
                column: "PurchaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_PurchaseNumber",
                table: "Purchases",
                column: "PurchaseNumber",
                unique: true);
        }
    }
}
