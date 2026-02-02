using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharma.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterUserTableAddFailedLoginTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "failed_login_attempts",
                schema: "pharma_identity",
                table: "user",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_failed_login_at",
                schema: "pharma_identity",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "locked_until",
                schema: "pharma_identity",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "failed_login_attempts",
                schema: "pharma_identity",
                table: "user");

            migrationBuilder.DropColumn(
                name: "last_failed_login_at",
                schema: "pharma_identity",
                table: "user");

            migrationBuilder.DropColumn(
                name: "locked_until",
                schema: "pharma_identity",
                table: "user");
        }
    }
}
