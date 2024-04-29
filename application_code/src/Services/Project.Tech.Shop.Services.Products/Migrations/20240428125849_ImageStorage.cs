using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Tech.Shop.Services.Products.Migrations
{
    /// <inheritdoc />
    public partial class ImageStorage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                schema: "Products",
                table: "Products",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                schema: "Products",
                table: "Products");
        }
    }
}
