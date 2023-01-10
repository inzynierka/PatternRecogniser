using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations_BackupDB
{
    public partial class addF1AndMissRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "F1",
                table: "ModelTrainingExperiment",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "F1",
                table: "ModelTrainingExperiment");
        }
    }
}
