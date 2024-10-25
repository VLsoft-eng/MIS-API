using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitWithBannedToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "banned_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_banned_tokens", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_banned_tokens_id",
                table: "banned_tokens",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_banned_tokens_token_value",
                table: "banned_tokens",
                column: "token_value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "banned_tokens");
        }
    }
}
