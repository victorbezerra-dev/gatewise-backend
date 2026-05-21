using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace GateWise.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameLabToSpace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_access_grants_labs_lab_id",
                table: "access_grants");

            migrationBuilder.DropForeignKey(
                name: "fk_access_logs_labs_lab_id",
                table: "access_logs");

            migrationBuilder.DropTable(
                name: "lab_access_managers");

            migrationBuilder.DropTable(
                name: "labs");

            migrationBuilder.RenameColumn(
                name: "lab_id",
                table: "access_logs",
                newName: "space_id");

            migrationBuilder.RenameIndex(
                name: "ix_access_logs_lab_id",
                table: "access_logs",
                newName: "ix_access_logs_space_id");

            migrationBuilder.RenameColumn(
                name: "lab_id",
                table: "access_grants",
                newName: "space_id");

            migrationBuilder.RenameIndex(
                name: "ix_access_grants_lab_id",
                table: "access_grants",
                newName: "ix_access_grants_space_id");

            migrationBuilder.CreateTable(
                name: "spaces",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    image_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    floor = table.Column<int>(type: "integer", nullable: false),
                    building = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    open_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_spaces", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "space_managers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    space_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_space_managers", x => x.id);
                    table.ForeignKey(
                        name: "fk_space_managers_spaces_space_id",
                        column: x => x.space_id,
                        principalTable: "spaces",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_space_managers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_space_managers_space_id",
                table: "space_managers",
                column: "space_id");

            migrationBuilder.CreateIndex(
                name: "ix_space_managers_user_id",
                table: "space_managers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_access_grants_spaces_space_id",
                table: "access_grants",
                column: "space_id",
                principalTable: "spaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_access_logs_spaces_space_id",
                table: "access_logs",
                column: "space_id",
                principalTable: "spaces",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_access_grants_spaces_space_id",
                table: "access_grants");

            migrationBuilder.DropForeignKey(
                name: "fk_access_logs_spaces_space_id",
                table: "access_logs");

            migrationBuilder.DropTable(
                name: "space_managers");

            migrationBuilder.DropTable(
                name: "spaces");

            migrationBuilder.RenameColumn(
                name: "space_id",
                table: "access_logs",
                newName: "lab_id");

            migrationBuilder.RenameIndex(
                name: "ix_access_logs_space_id",
                table: "access_logs",
                newName: "ix_access_logs_lab_id");

            migrationBuilder.RenameColumn(
                name: "space_id",
                table: "access_grants",
                newName: "lab_id");

            migrationBuilder.RenameIndex(
                name: "ix_access_grants_space_id",
                table: "access_grants",
                newName: "ix_access_grants_lab_id");

            migrationBuilder.CreateTable(
                name: "labs",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    building = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    capacity = table.Column<int>(type: "integer", nullable: false),
                    close_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    floor = table.Column<int>(type: "integer", nullable: false),
                    imagem_url = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    location = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    open_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lab_access_managers",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    lab_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<string>(type: "varchar(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lab_access_managers", x => x.id);
                    table.ForeignKey(
                        name: "fk_lab_access_managers_labs_lab_id",
                        column: x => x.lab_id,
                        principalTable: "labs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_lab_access_managers_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_lab_access_managers_lab_id",
                table: "lab_access_managers",
                column: "lab_id");

            migrationBuilder.CreateIndex(
                name: "ix_lab_access_managers_user_id",
                table: "lab_access_managers",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "fk_access_grants_labs_lab_id",
                table: "access_grants",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_access_logs_labs_lab_id",
                table: "access_logs",
                column: "lab_id",
                principalTable: "labs",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
