using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace cinemate.Migrations
{
    /// <inheritdoc />
    public partial class SubCategories : Migration
    {
        internal Guid Id;
        internal string Name;
        internal string Description;
        internal Guid ParentCategoryId;
        internal string Picture;
        internal int ContentCount;
        internal string ParentCategoryName;

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubCategoriesEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Picture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubCategoriesEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubCategoriesEntity_Gategories_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Gategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubCategoriesEntity_ParentCategoryId",
                table: "SubCategoriesEntity",
                column: "ParentCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubCategoriesEntity");
        }
    }
}
