using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kronos.WebAPI.Migrations.Hermes
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hermes");

            migrationBuilder.CreateTable(
                name: "TokenCryptoData",
                schema: "hermes",
                columns: table => new
                {
                    EntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    SigningKey = table.Column<byte[]>(type: "bytea", maxLength: 0, nullable: false),
                    EncryptionKey = table.Column<byte[]>(type: "bytea", maxLength: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenCryptoData", x => x.EntityId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TokenCryptoData",
                schema: "hermes");
        }
    }
}
