using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kronos.WebAPI.Migrations.Athena
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
                name: "ServiceAccounts",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    AuthorizationCode = table.Column<byte[]>(type: "bytea", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceScopes",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAccounts",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceId = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Username = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserScopes",
                schema: "athena",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserScopes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServicesScopes",
                schema: "athena",
                columns: table => new
                {
                    AccountsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopesId = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicesScopes", x => new { x.AccountsId, x.ScopesId });
                    table.ForeignKey(
                        name: "FK_ServicesScopes_ServiceAccounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "athena",
                        principalTable: "ServiceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicesScopes_ServiceAccounts_AccountsId",
                        column: x => x.AccountsId,
                        principalSchema: "athena",
                        principalTable: "ServiceAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicesScopes_ServiceScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "athena",
                        principalTable: "ServiceScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicesScopes_ServiceScopes_ScopesId",
                        column: x => x.ScopesId,
                        principalSchema: "athena",
                        principalTable: "ServiceScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersScopes",
                schema: "athena",
                columns: table => new
                {
                    AccountsId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopesId = table.Column<string>(type: "character varying(128)", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ScopeId = table.Column<string>(type: "character varying(128)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersScopes", x => new { x.AccountsId, x.ScopesId });
                    table.ForeignKey(
                        name: "FK_UsersScopes_UserAccounts_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "athena",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersScopes_UserAccounts_AccountsId",
                        column: x => x.AccountsId,
                        principalSchema: "athena",
                        principalTable: "UserAccounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersScopes_UserScopes_ScopeId",
                        column: x => x.ScopeId,
                        principalSchema: "athena",
                        principalTable: "UserScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersScopes_UserScopes_ScopesId",
                        column: x => x.ScopesId,
                        principalSchema: "athena",
                        principalTable: "UserScopes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServicesScopes_AccountId",
                schema: "athena",
                table: "ServicesScopes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesScopes_ScopeId",
                schema: "athena",
                table: "ServicesScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicesScopes_ScopesId",
                schema: "athena",
                table: "ServicesScopes",
                column: "ScopesId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersScopes_AccountId",
                schema: "athena",
                table: "UsersScopes",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersScopes_ScopeId",
                schema: "athena",
                table: "UsersScopes",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersScopes_ScopesId",
                schema: "athena",
                table: "UsersScopes",
                column: "ScopesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServicesScopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "UsersScopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "ServiceAccounts",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "ServiceScopes",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "UserAccounts",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "UserScopes",
                schema: "athena");
        }
    }
}
