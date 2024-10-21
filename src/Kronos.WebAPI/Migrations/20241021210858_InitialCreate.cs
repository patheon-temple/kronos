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
                name: "service_accounts",
                schema: "athena",
                columns: table => new
                {
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    secret = table.Column<byte[]>(type: "bytea", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_service_account_id", x => x.service_id);
                });

            migrationBuilder.CreateTable(
                name: "user_accounts",
                schema: "athena",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_account_id", x => x.user_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "service_accounts",
                schema: "athena");

            migrationBuilder.DropTable(
                name: "user_accounts",
                schema: "athena");
        }
    }
}
