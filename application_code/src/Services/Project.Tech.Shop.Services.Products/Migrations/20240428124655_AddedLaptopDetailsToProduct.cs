using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Tech.Shop.Services.Products.Migrations
{
    /// <inheritdoc />
    public partial class AddedLaptopDetailsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CancelDate",
                schema: "Products",
                table: "Sales",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "Products",
                table: "Sales",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "Category",
                schema: "Products",
                table: "Products",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Condition",
                schema: "Products",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "GraphicsCard",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProcessorType",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RAM",
                schema: "Products",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ScreenSize",
                schema: "Products",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Series",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Storage",
                schema: "Products",
                table: "Products",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageType",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TouchScreen",
                schema: "Products",
                table: "Products",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancelDate",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "Products",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Brand",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Condition",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "GraphicsCard",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProcessorType",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RAM",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ScreenSize",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Series",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Storage",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StorageType",
                schema: "Products",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "TouchScreen",
                schema: "Products",
                table: "Products");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "Products",
                table: "Products",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
