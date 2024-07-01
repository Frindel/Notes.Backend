using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Notes.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "text", maxLength: 20, nullable: false),
                    password = table.Column<string>(type: "text", maxLength: 256, nullable: false),
                    refresh_token = table.Column<string>(type: "text", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "integer", nullable: false),
                    personal_id = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 30, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => new { x.category_id, x.personal_id });
                    table.ForeignKey(
                        name: "FK_categories_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "notes",
                columns: table => new
                {
                    note_id = table.Column<int>(type: "integer", nullable: false),
                    personal_id = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", maxLength: 80, nullable: false),
                    description = table.Column<string>(type: "text", maxLength: 400, nullable: false),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notes", x => new { x.note_id, x.personal_id });
                    table.ForeignKey(
                        name: "FK_notes_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "note_categories",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "integer", nullable: false),
                    CategoriesPersonalId = table.Column<int>(type: "integer", nullable: false),
                    NotesId = table.Column<int>(type: "integer", nullable: false),
                    NotesPersonalId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_note_categories", x => new { x.CategoriesId, x.CategoriesPersonalId, x.NotesId, x.NotesPersonalId });
                    table.ForeignKey(
                        name: "FK_note_categories_categories_CategoriesId_CategoriesPersonalId",
                        columns: x => new { x.CategoriesId, x.CategoriesPersonalId },
                        principalTable: "categories",
                        principalColumns: new[] { "category_id", "personal_id" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_note_categories_notes_NotesId_NotesPersonalId",
                        columns: x => new { x.NotesId, x.NotesPersonalId },
                        principalTable: "notes",
                        principalColumns: new[] { "note_id", "personal_id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categories_UserId",
                table: "categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_note_categories_NotesId_NotesPersonalId",
                table: "note_categories",
                columns: new[] { "NotesId", "NotesPersonalId" });

            migrationBuilder.CreateIndex(
                name: "IX_notes_UserId",
                table: "notes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "note_categories");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "notes");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
