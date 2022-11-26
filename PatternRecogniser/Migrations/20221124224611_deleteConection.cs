using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class deleteConection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_experiment_extendedModel_extendedModelId1",
                table: "experiment");

            migrationBuilder.DropForeignKey(
                name: "FK_PatternRecognitionExperiment_extendedModel_extendedModelId2",
                table: "PatternRecognitionExperiment");

            migrationBuilder.DropIndex(
                name: "IX_PatternRecognitionExperiment_extendedModelId2",
                table: "PatternRecognitionExperiment");

            migrationBuilder.DropIndex(
                name: "IX_experiment_extendedModelId1",
                table: "experiment");

            migrationBuilder.DropColumn(
                name: "extendedModelId2",
                table: "PatternRecognitionExperiment");

            migrationBuilder.DropColumn(
                name: "extendedModelId1",
                table: "experiment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "extendedModelId2",
                table: "PatternRecognitionExperiment",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "extendedModelId1",
                table: "experiment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatternRecognitionExperiment_extendedModelId2",
                table: "PatternRecognitionExperiment",
                column: "extendedModelId2");

            migrationBuilder.CreateIndex(
                name: "IX_experiment_extendedModelId1",
                table: "experiment",
                column: "extendedModelId1");

            migrationBuilder.AddForeignKey(
                name: "FK_experiment_extendedModel_extendedModelId1",
                table: "experiment",
                column: "extendedModelId1",
                principalTable: "extendedModel",
                principalColumn: "extendedModelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatternRecognitionExperiment_extendedModel_extendedModelId2",
                table: "PatternRecognitionExperiment",
                column: "extendedModelId2",
                principalTable: "extendedModel",
                principalColumn: "extendedModelId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
