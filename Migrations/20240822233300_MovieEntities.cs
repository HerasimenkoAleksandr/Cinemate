using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class MovieEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "dislikeCount",
                table: "MoviesEntities",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "likeCount",
                table: "MoviesEntities",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dislikeCount",
                table: "MoviesEntities");

            migrationBuilder.DropColumn(
                name: "likeCount",
                table: "MoviesEntities");
        }
    }
}
