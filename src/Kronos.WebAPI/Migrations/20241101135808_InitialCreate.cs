using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kronos.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "athena");

            migrationBuilder.CreateTable(
                name: "Scopes",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAccounts",
                schema: "athena",
                columns: table => new
                {
                    ServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Secret = table.Column<byte[]>(type: "bytea", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAccounts", x => x.ServiceId);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                schema: "athena",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UsersScopes",
                schema: "athena",
                columns: table => new
                {
                    ScopesId = table.Column<string>(type: "character varying(128)", nullable: false),
                    UserAccountsUserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersScopes", x => new { x.ScopesId, x.UserAccountsUserId });
                    table.ForeignKey(
                        name: "FK_UsersScopes_Scopes_ScopesId",
                        column: x => x.ScopesId,
                        principalSchema: "athena",
                        principalTable: "Scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersScopes_UserAccounts_UserAccountsUserId",
                        column: x => x.UserAccountsUserId,
                        principalSchema: "athena",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersScopes_UserAccountsUserId",
                schema: "athena",
                table: "UsersScopes",
                column: "UserAccountsUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceAccounts",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "UsersScopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "Scopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "UserAccounts",
                schema: "athena");
        }
    }
}
