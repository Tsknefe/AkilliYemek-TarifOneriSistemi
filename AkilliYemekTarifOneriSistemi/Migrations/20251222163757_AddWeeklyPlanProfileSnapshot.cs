using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliYemekTarifOneriSistemi.Migrations
{
    
    public partial class AddWeeklyPlanProfileSnapshot : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DietTypeSnapshot",
                table: "WeeklyPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "TargetCaloriesSnapshot",
                table: "WeeklyPlans",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietTypeSnapshot",
                table: "WeeklyPlans");

            migrationBuilder.DropColumn(
                name: "TargetCaloriesSnapshot",
                table: "WeeklyPlans");
        }
    }
}
