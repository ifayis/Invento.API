using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invento.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Salesmodule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId1",
                table: "SaleItems",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_ProductId1",
                table: "SaleItems",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Products_ProductId1",
                table: "SaleItems",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_Products_ProductId1",
                table: "SaleItems");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_ProductId1",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "SaleItems");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_Products_ProductId",
                table: "SaleItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
