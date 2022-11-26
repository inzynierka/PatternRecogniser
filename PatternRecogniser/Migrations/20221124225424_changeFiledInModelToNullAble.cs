using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class changeFiledInModelToNullAble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_extendedModel_ModelTrainingExperiment_modelTrainingExperime~",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_extendedModel_modelTrainingExperimentId",
                table: "extendedModel");

            migrationBuilder.DropColumn(
                name: "modelTrainingExperimentId",
                table: "extendedModel");

            migrationBuilder.AddColumn<int>(
                name: "extendedModelId1",
                table: "ModelTrainingExperiment",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelTrainingExperiment_extendedModelId1",
                table: "ModelTrainingExperiment",
                column: "extendedModelId1",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTrainingExperiment_extendedModel_extendedModelId1",
                table: "ModelTrainingExperiment",
                column: "extendedModelId1",
                principalTable: "extendedModel",
                principalColumn: "extendedModelId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelTrainingExperiment_extendedModel_extendedModelId1",
                table: "ModelTrainingExperiment");

            migrationBuilder.DropIndex(
                name: "IX_ModelTrainingExperiment_extendedModelId1",
                table: "ModelTrainingExperiment");

            migrationBuilder.DropColumn(
                name: "extendedModelId1",
                table: "ModelTrainingExperiment");

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

            migrationBuilder.AddForeignKey(
                name: "FK_extendedModel_ModelTrainingExperiment_modelTrainingExperime~",
                table: "extendedModel",
                column: "modelTrainingExperimentId",
                principalTable: "ModelTrainingExperiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
