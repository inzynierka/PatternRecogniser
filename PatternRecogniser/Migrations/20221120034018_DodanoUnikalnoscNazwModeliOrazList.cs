using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class DodanoUnikalnoscNazwModeliOrazList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperimentExperimentList_experimentList_experimentListId",
                table: "ExperimentExperimentList");

            migrationBuilder.DropForeignKey(
                name: "FK_experimentList_user_userID",
                table: "experimentList");

            migrationBuilder.DropForeignKey(
                name: "FK_extendedModel_user_userId",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_extendedModel_userId",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_experimentList_userID",
                table: "experimentList");

            migrationBuilder.RenameColumn(
                name: "userID",
                table: "experimentList",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "experimentListId",
                table: "ExperimentExperimentList",
                newName: "experimentListsexperimentListId");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "extendedModel",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_extendedModel_userId_name",
                table: "extendedModel",
                columns: new[] { "userId", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_experimentList_userId_name",
                table: "experimentList",
                columns: new[] { "userId", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ExperimentExperimentList_experimentList_experimentListsexpe~",
                table: "ExperimentExperimentList",
                column: "experimentListsexperimentListId",
                principalTable: "experimentList",
                principalColumn: "experimentListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_experimentList_user_userId",
                table: "experimentList",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_extendedModel_user_userId",
                table: "extendedModel",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExperimentExperimentList_experimentList_experimentListsexpe~",
                table: "ExperimentExperimentList");

            migrationBuilder.DropForeignKey(
                name: "FK_experimentList_user_userId",
                table: "experimentList");

            migrationBuilder.DropForeignKey(
                name: "FK_extendedModel_user_userId",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_extendedModel_userId_name",
                table: "extendedModel");

            migrationBuilder.DropIndex(
                name: "IX_experimentList_userId_name",
                table: "experimentList");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "experimentList",
                newName: "userID");

            migrationBuilder.RenameColumn(
                name: "experimentListsexperimentListId",
                table: "ExperimentExperimentList",
                newName: "experimentListId");

            migrationBuilder.AlterColumn<int>(
                name: "userId",
                table: "extendedModel",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_extendedModel_userId",
                table: "extendedModel",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_experimentList_userID",
                table: "experimentList",
                column: "userID");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperimentExperimentList_experimentList_experimentListId",
                table: "ExperimentExperimentList",
                column: "experimentListId",
                principalTable: "experimentList",
                principalColumn: "experimentListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_experimentList_user_userID",
                table: "experimentList",
                column: "userID",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_extendedModel_user_userId",
                table: "extendedModel",
                column: "userId",
                principalTable: "user",
                principalColumn: "userId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
