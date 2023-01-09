using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations_BackupDB
{
    public partial class RocJson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "serializedRoc",
                table: "ModelTrainingExperiment",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "serializedRoc",
                table: "ModelTrainingExperiment");
        }
    }
}
