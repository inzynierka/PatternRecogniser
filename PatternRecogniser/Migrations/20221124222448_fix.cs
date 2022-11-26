using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class fix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "modelTrainingExperimentId",
                table: "extendedModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_extendedModel_modelTrainingExperimentId",
                table: "extendedModel",
                column: "modelTrainingExperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_experiment_extendedModelId",
                table: "experiment",
                column: "extendedModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_experiment_extendedModel_extendedModelId",
                table: "experiment",
                column: "extendedModelId",
                principalTable: "extendedModel",
                principalColumn: "extendedModelId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_extendedModel_ModelTrainingExperiment_modelTrainingExperime~",
                table: "extendedModel",
                column: "modelTrainingExperimentId",
                principalTable: "ModelTrainingExperiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_experiment_extendedModel_extendedModelId",
                table: "experiment");

            migrationBuilder.DropForeignKey(
                name: "FK_extendedModel_ModelTrainingExperiment_modelTrainingExperime~",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_extendedModel_modelTrainingExperimentId",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_experiment_extendedModelId",
                table: "experiment");

            migrationBuilder.DropColumn(
                name: "modelTrainingExperimentId",
                table: "extendedModel");
        }
    }
}
