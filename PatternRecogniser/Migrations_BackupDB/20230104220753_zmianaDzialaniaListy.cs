using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations_BackupDB
{
    public partial class zmianaDzialaniaListy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recognisedPatterns_PatternRecognitionExperiment_PatternReco~",
                table: "recognisedPatterns");

            migrationBuilder.DropColumn(
                name: "exsistUnsavePatternRecognitionExperiment",
                table: "user");

            migrationBuilder.AlterColumn<int>(
                name: "PatternRecognitionExperimentexperimentId",
                table: "recognisedPatterns",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_recognisedPatterns_PatternRecognitionExperiment_PatternReco~",
                table: "recognisedPatterns",
                column: "PatternRecognitionExperimentexperimentId",
                principalTable: "PatternRecognitionExperiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recognisedPatterns_PatternRecognitionExperiment_PatternReco~",
                table: "recognisedPatterns");

            migrationBuilder.AddColumn<bool>(
                name: "exsistUnsavePatternRecognitionExperiment",
                table: "user",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "PatternRecognitionExperimentexperimentId",
                table: "recognisedPatterns",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_recognisedPatterns_PatternRecognitionExperiment_PatternReco~",
                table: "recognisedPatterns",
                column: "PatternRecognitionExperimentexperimentId",
                principalTable: "PatternRecognitionExperiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
