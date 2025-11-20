using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliYemekTarifOneriSistemi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIngredientNutritionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Calories100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Carbs100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Fat100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Fiber100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Protein100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Sodium100g",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Sugar100g",
                table: "Ingredients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Calories100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Carbs100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Fat100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Fiber100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Protein100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Sodium100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Sugar100g",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
