using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kronos.WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "password_hash",
                schema: "athena",
                table: "user_accounts",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "username",
                schema: "athena",
                table: "user_accounts",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "password_hash",
                schema: "athena",
                table: "user_accounts");

            migrationBuilder.DropColumn(
                name: "username",
                schema: "athena",
                table: "user_accounts");
        }
    }
}
