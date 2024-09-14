using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class LikeForMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LikeForMovie",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MovieId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsLiked = table.Column<bool>(type: "bit", nullable: false),
                    IsDisliked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LikeForMovie", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LikeForMovie_MoviesEntities_MovieId",
                        column: x => x.MovieId,
                        principalTable: "MoviesEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LikeForMovie_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LikeForMovie_MovieId",
                table: "LikeForMovie",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_LikeForMovie_UserId",
                table: "LikeForMovie",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LikeForMovie");
        }
    }
}
