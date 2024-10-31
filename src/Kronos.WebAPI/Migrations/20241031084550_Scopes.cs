using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kronos.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class Scopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "scopes",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_scope_id", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users_scopes",
                schema: "athena",
                columns: table => new
                {
                    ScopesId = table.Column<string>(type: "character varying(128)", nullable: false),
                    UserAccountsUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users_scopes", x => new { x.ScopesId, x.UserAccountsUserId });
                    table.ForeignKey(
                        name: "FK_users_scopes_scopes_ScopesId",
                        column: x => x.ScopesId,
                        principalSchema: "athena",
                        principalTable: "scopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_users_scopes_user_accounts_UserAccountsUserId",
                        column: x => x.UserAccountsUserId,
                        principalSchema: "athena",
                        principalTable: "user_accounts",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_scopes_UserAccountsUserId",
                schema: "athena",
                table: "users_scopes",
                column: "UserAccountsUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users_scopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "scopes",
                schema: "athena");
        }
    }
}
