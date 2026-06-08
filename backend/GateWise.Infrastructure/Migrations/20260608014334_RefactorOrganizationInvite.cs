using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RefactorOrganizationInvite : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                table: "organization_invites");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "organization_invites",
                newName: "uses_count");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expires_at",
                table: "organization_invites",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "organization_invites",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "max_uses",
                table: "organization_invites",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "organization_invites");

            migrationBuilder.DropColumn(
                name: "max_uses",
                table: "organization_invites");

            migrationBuilder.RenameColumn(
                name: "uses_count",
                table: "organization_invites",
                newName: "status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "expires_at",
                table: "organization_invites",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "email",
                table: "organization_invites",
                type: "text",
                nullable: true);
        }
    }
}
