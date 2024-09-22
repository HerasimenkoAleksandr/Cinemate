using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class AddComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommentMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUsers = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdMovie = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CommentDt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeleteCommentDt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentMovies_MoviesEntities_IdMovie",
                        column: x => x.IdMovie,
                        principalTable: "MoviesEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentMovies_Users_IdUsers",
                        column: x => x.IdUsers,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommentMovies_IdMovie",
                table: "CommentMovies",
                column: "IdMovie");

            migrationBuilder.CreateIndex(
                name: "IX_CommentMovies_IdUsers",
                table: "CommentMovies",
                column: "IdUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentMovies");
        }
    }
}
