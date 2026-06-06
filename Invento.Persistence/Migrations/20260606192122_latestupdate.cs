using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class latestupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriticalStockThreshold",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "LowStockThreshold",
                table: "TenantSettings");

            migrationBuilder.AddColumn<int>(
                name: "CurrentStockAfterMovement",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentStockAfterMovement",
                table: "StockMovements");

            migrationBuilder.AddColumn<int>(
                name: "CriticalStockThreshold",
                table: "TenantSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LowStockThreshold",
                table: "TenantSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
