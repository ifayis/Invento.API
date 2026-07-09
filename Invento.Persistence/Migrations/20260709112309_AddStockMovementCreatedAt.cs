using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovementCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantId_Name",
                table: "Suppliers");

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
                name: "IX_Sales_TenantId_IsDeleted_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_Name",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_CashTransactions_TenantId_TransactionDate",
                table: "CashTransactions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE StockMovements
                SET CreatedAt = SYSUTCDATETIME()
                WHERE CreatedAt IS NULL;
            ");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
                migrationBuilder.CreateTable(
                name: "DocumentNumberSequences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PeriodKey = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NextNumber = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNumberSequences", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId_IsDeleted_FullName_Id",
                table: "Users",
                columns: new[] { "TenantId", "IsDeleted", "FullName", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId_IsDeleted_CreatedAt_Id",
                table: "Suppliers",
                columns: new[] { "TenantId", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_IsDeleted_CreatedAt_Id",
                table: "StockMovements",
                columns: new[] { "TenantId", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_MovementType_IsDeleted_CreatedAt_Id",
                table: "StockMovements",
                columns: new[] { "TenantId", "MovementType", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_ProductId_IsDeleted_CreatedAt_Id",
                table: "StockMovements",
                columns: new[] { "TenantId", "ProductId", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_CustomerId_IsDeleted_SaleDate_Id",
                table: "Sales",
                columns: new[] { "TenantId", "CustomerId", "IsDeleted", "SaleDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_TenantId_IsDeleted_SaleDate_Id",
                table: "Sales",
                columns: new[] { "TenantId", "IsDeleted", "SaleDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate_Id",
                table: "Purchases",
                columns: new[] { "TenantId", "IsDeleted", "PurchaseDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate_Id",
                table: "Purchases",
                columns: new[] { "TenantId", "SupplierId", "IsDeleted", "PurchaseDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_IsDeleted_CreatedAt_Id",
                table: "Products",
                columns: new[] { "TenantId", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId_IsDeleted_Name_Id",
                table: "Customers",
                columns: new[] { "TenantId", "IsDeleted", "Name", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TenantId_IsDeleted_CreatedAt_Id",
                table: "Categories",
                columns: new[] { "TenantId", "IsDeleted", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_TenantId_IsDeleted_TransactionDate_Id",
                table: "CashTransactions",
                columns: new[] { "TenantId", "IsDeleted", "TransactionDate", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumberSequences_TenantId_DocumentType_PeriodKey",
                table: "DocumentNumberSequences",
                columns: new[] { "TenantId", "DocumentType", "PeriodKey" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentNumberSequences");

            migrationBuilder.DropIndex(
                name: "IX_Users_TenantId_IsDeleted_FullName_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Suppliers_TenantId_IsDeleted_CreatedAt_Id",
                table: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_IsDeleted_CreatedAt_Id",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_MovementType_IsDeleted_CreatedAt_Id",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_ProductId_IsDeleted_CreatedAt_Id",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_CustomerId_IsDeleted_SaleDate_Id",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_TenantId_IsDeleted_SaleDate_Id",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate_Id",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate_Id",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_IsDeleted_CreatedAt_Id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Customers_TenantId_IsDeleted_Name_Id",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TenantId_IsDeleted_CreatedAt_Id",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_CashTransactions_TenantId_IsDeleted_TransactionDate_Id",
                table: "CashTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockMovements");

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_TenantId_Name",
                table: "Suppliers",
                columns: new[] { "TenantId", "Name" });

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
                name: "IX_Sales_TenantId_IsDeleted_SaleDate",
                table: "Sales",
                columns: new[] { "TenantId", "IsDeleted", "SaleDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_IsDeleted_PurchaseDate",
                table: "Purchases",
                columns: new[] { "TenantId", "IsDeleted", "PurchaseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_TenantId_SupplierId_IsDeleted_PurchaseDate",
                table: "Purchases",
                columns: new[] { "TenantId", "SupplierId", "IsDeleted", "PurchaseDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_Name",
                table: "Products",
                columns: new[] { "TenantId", "Name" });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_TenantId",
                table: "Customers",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_CashTransactions_TenantId_TransactionDate",
                table: "CashTransactions",
                columns: new[] { "TenantId", "TransactionDate" });
        }
    }
}
