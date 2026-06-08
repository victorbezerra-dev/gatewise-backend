using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsInstitutionalFromOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_institutional",
                table: "organizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_institutional",
                table: "organizations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
