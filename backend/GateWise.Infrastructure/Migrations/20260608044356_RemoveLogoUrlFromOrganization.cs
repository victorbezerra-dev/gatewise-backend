using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLogoUrlFromOrganization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "logo_url",
                table: "organizations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "logo_url",
                table: "organizations",
                type: "text",
                nullable: true);
        }
    }
}
