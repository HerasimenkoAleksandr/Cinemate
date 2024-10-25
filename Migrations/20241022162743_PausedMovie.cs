using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class PausedMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PausedMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PauseTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PausedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PausedMovies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PausedMovies_MoviesEntities_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MoviesEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PausedMovies_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PausedMovies_MovieId",
                table: "PausedMovies",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_PausedMovies_UserId",
                table: "PausedMovies",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PausedMovies");
        }
    }
}
