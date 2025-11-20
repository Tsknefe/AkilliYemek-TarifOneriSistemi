using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliYemekTarifOneriSistemi.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEnglishNameToIngredient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "Ingredients");
        }
    }
}
