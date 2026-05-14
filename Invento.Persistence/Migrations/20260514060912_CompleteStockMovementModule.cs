using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteStockMovementModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StockMovements_CreatedAt",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_TenantId_ProductId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceType",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "StockMovements");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "StockMovements",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "StockMovements",
                newName: "Remarks");

            migrationBuilder.AddColumn<string>(
                name: "MovementType",
                table: "StockMovements",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "StockMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovementType",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "CurrentStock",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Remarks",
                table: "StockMovements",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "StockMovements",
                newName: "ReferenceId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceType",
                table: "StockMovements",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "StockMovements",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "StockMovements",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "StockMovements",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CreatedAt",
                table: "StockMovements",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_TenantId_ProductId",
                table: "StockMovements",
                columns: new[] { "TenantId", "ProductId" });
        }
    }
}
