using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharma.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pharma_identity");

            migrationBuilder.CreateTable(
                name: "user",
                schema: "pharma_identity",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "varchar(26)", maxLength: 26, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    is_two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    totp_secret_encrypted = table.Column<string>(type: "text", nullable: true),
                    is_account_locked = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "text", nullable: true),
                    updated_by = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.user_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user",
                schema: "pharma_identity");
        }
    }
}
