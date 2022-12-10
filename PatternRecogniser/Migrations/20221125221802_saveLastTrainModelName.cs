using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class saveLastTrainModelName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExsistUnsavePatternRecognitionExperiment",
                table: "user",
                newName: "exsistUnsavePatternRecognitionExperiment");

            migrationBuilder.AddColumn<string>(
                name: "lastTrainModelName",
                table: "user",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastTrainModelName",
                table: "user");

            migrationBuilder.RenameColumn(
                name: "exsistUnsavePatternRecognitionExperiment",
                table: "user",
                newName: "ExsistUnsavePatternRecognitionExperiment");
        }
    }
}
