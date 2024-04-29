using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Tech.Shop.Services.Products.Migrations
{
    /// <inheritdoc />
    public partial class RefinedBasketsandAndsaleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Products_ProductId",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_CustomerId",
                schema: "Products",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "CancelDate",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "QuantitySold",
                schema: "Products",
                table: "Sales");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                schema: "Products",
                table: "Sales",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<Guid>(
                name: "BasketId",
                schema: "Products",
                table: "Sales",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "Products",
                table: "Baskets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_BasketId",
                schema: "Products",
                table: "Sales",
                column: "BasketId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Baskets_BasketId",
                schema: "Products",
                table: "Sales",
                column: "BasketId",
                principalSchema: "Products",
                principalTable: "Baskets",
                principalColumn: "BasketId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Products_ProductId",
                schema: "Products",
                table: "Sales",
                column: "ProductId",
                principalSchema: "Products",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Baskets_BasketId",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Products_ProductId",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_BasketId",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "BasketId",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Products",
                table: "Baskets");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                schema: "Products",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                schema: "Products",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "QuantitySold",
                schema: "Products",
                table: "Sales",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_CustomerId",
                schema: "Products",
                table: "Baskets",
                column: "CustomerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Products_ProductId",
                schema: "Products",
                table: "Sales",
                column: "ProductId",
                principalSchema: "Products",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
