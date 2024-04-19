using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Tech.Shop.Services.UsersAccounts.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "UserAccounts");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "UserAccounts",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "UserAccounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    Username = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "AccountActivities",
                schema: "UserAccounts",
                columns: table => new
                {
                    ActivityId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    ActivityType = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountActivities", x => x.ActivityId);
                    table.ForeignKey(
                        name: "FK_AccountActivities_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "UserAccounts",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "UserAccounts",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddressLine = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    City = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    State = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    ZipCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Country = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    IsShippingAddress = table.Column<bool>(type: "boolean", nullable: false),
                    IsBillingAddress = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "UserAccounts",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SecuritySettings",
                schema: "UserAccounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    SecurityQuestion = table.Column<string>(type: "text", nullable: false),
                    SecurityAnswerHash = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecuritySettings", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_SecuritySettings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "UserAccounts",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPreferences",
                schema: "UserAccounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReceiveNewsletter = table.Column<bool>(type: "boolean", nullable: false),
                    PreferredPaymentMethod = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferences", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserPreferences_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "UserAccounts",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "UserAccounts",
                columns: table => new
                {
                    RolesRoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    UsersUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.RolesRoleId, x.UsersUserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RolesRoleId",
                        column: x => x.RolesRoleId,
                        principalSchema: "UserAccounts",
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UsersUserId",
                        column: x => x.UsersUserId,
                        principalSchema: "UserAccounts",
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountActivities_UserId",
                schema: "UserAccounts",
                table: "AccountActivities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                schema: "UserAccounts",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UsersUserId",
                schema: "UserAccounts",
                table: "UserRoles",
                column: "UsersUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "UserAccounts",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "UserAccounts",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountActivities",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "SecuritySettings",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "UserPreferences",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "UserAccounts");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "UserAccounts");
        }
    }
}
