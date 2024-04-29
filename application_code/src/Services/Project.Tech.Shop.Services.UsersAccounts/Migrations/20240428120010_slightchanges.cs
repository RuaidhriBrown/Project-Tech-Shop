using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Tech.Shop.Services.UsersAccounts.Migrations
{
    /// <inheritdoc />
    public partial class slightchanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ZipCode",
                schema: "UserAccounts",
                table: "Addresses",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "State",
                schema: "UserAccounts",
                table: "Addresses",
                newName: "County");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostCode",
                schema: "UserAccounts",
                table: "Addresses",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "County",
                schema: "UserAccounts",
                table: "Addresses",
                newName: "State");
        }
    }
}
