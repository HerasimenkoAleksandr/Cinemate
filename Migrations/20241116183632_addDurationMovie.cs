using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class addDurationMovie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "MoviesEntities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "MoviesEntities");
        }
    }
}
