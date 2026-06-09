using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMemberAccessPeriod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "expires_at",
                table: "organization_members",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "starts_at",
                table: "organization_members",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "member_expires_at",
                table: "organization_invites",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "member_starts_at",
                table: "organization_invites",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expires_at",
                table: "organization_members");

            migrationBuilder.DropColumn(
                name: "starts_at",
                table: "organization_members");

            migrationBuilder.DropColumn(
                name: "member_expires_at",
                table: "organization_invites");

            migrationBuilder.DropColumn(
                name: "member_starts_at",
                table: "organization_invites");
        }
    }
}
