using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pharma.Identity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterTableUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hashed_backup_codes",
                schema: "pharma_identity",
                table: "user",
                type: "jsonb",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hashed_backup_codes",
                schema: "pharma_identity",
                table: "user");
        }
    }
}
