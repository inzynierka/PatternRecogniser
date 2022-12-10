using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class saveLastExperiment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ExsistUnsavePatternRecognitionExperiment",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "lastPatternRecognitionExperimentexperimentId",
                table: "user",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_lastPatternRecognitionExperimentexperimentId",
                table: "user",
                column: "lastPatternRecognitionExperimentexperimentId");

            migrationBuilder.AddForeignKey(
                name: "FK_user_PatternRecognitionExperiment_lastPatternRecognitionExp~",
                table: "user",
                column: "lastPatternRecognitionExperimentexperimentId",
                principalTable: "PatternRecognitionExperiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_user_PatternRecognitionExperiment_lastPatternRecognitionExp~",
                table: "user");

            migrationBuilder.DropIndex(
                name: "IX_user_lastPatternRecognitionExperimentexperimentId",
                table: "user");

            migrationBuilder.DropColumn(
                name: "ExsistUnsavePatternRecognitionExperiment",
                table: "user");

            migrationBuilder.DropColumn(
                name: "lastPatternRecognitionExperimentexperimentId",
                table: "user");
        }
    }
}
