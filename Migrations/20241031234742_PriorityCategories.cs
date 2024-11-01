using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class PriorityCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PriorityCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriorityCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PriorityCategories_Gategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Gategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PriorityCategories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PriorityCategories_CategoryId",
                table: "PriorityCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PriorityCategories_UserId",
                table: "PriorityCategories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriorityCategories");
        }
    }
}
