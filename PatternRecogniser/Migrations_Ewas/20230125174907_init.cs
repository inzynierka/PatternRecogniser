using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PatternRecogniser.EwaMigrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExperimentExperimentList",
                columns: table => new
                {
                    experimentListsexperimentListId = table.Column<int>(type: "integer", nullable: false),
                    experimentsexperimentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentExperimentList", x => new { x.experimentListsexperimentListId, x.experimentsexperimentId });
                });

            migrationBuilder.CreateTable(
                name: "ModelTrainingExperiment",
                columns: table => new
                {
                    experimentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    accuracy = table.Column<double>(type: "double precision", nullable: false),
                    precision = table.Column<double>(type: "double precision", nullable: false),
                    recall = table.Column<double>(type: "double precision", nullable: false),
                    specificity = table.Column<double>(type: "double precision", nullable: false),
                    missRate = table.Column<double>(type: "double precision", nullable: false),
                    F1 = table.Column<double>(type: "double precision", nullable: false),
                    confusionMatrix = table.Column<int[]>(type: "integer[]", nullable: true),
                    serializedRoc = table.Column<string>(type: "text", nullable: true),
                    extendedModelId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTrainingExperiment", x => x.experimentId);
                });

            migrationBuilder.CreateTable(
                name: "PatternRecognitionExperiment",
                columns: table => new
                {
                    experimentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    testedPattern = table.Column<byte[]>(type: "bytea", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatternRecognitionExperiment", x => x.experimentId);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    login = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    hashedPassword = table.Column<string>(type: "text", nullable: true),
                    lastTrainModelName = table.Column<string>(type: "text", nullable: true),
                    lastCheckModel = table.Column<string>(type: "text", nullable: true),
                    lastModelStatus = table.Column<int>(type: "integer", nullable: false),
                    lastPatternRecognitionExperimentexperimentId = table.Column<int>(type: "integer", nullable: true),
                    createDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    lastLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    refreshToken = table.Column<string>(type: "text", nullable: true),
                    refreshTokenExpiryDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.login);
                    table.ForeignKey(
                        name: "FK_user_PatternRecognitionExperiment_lastPatternRecognitionExp~",
                        column: x => x.lastPatternRecognitionExperimentexperimentId,
                        principalTable: "PatternRecognitionExperiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "experimentList",
                columns: table => new
                {
                    experimentListId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    experimentType = table.Column<string>(type: "text", nullable: true),
                    userLogin = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_experimentList", x => x.experimentListId);
                    table.ForeignKey(
                        name: "FK_experimentList_user_userLogin",
                        column: x => x.userLogin,
                        principalTable: "user",
                        principalColumn: "login",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "extendedModel",
                columns: table => new
                {
                    extendedModelId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userLogin = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: true),
                    distribution = table.Column<int>(type: "integer", nullable: false),
                    modelInBytes = table.Column<byte[]>(type: "bytea", nullable: true),
                    num_classes = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_extendedModel", x => x.extendedModelId);
                    table.ForeignKey(
                        name: "FK_extendedModel_user_userLogin",
                        column: x => x.userLogin,
                        principalTable: "user",
                        principalColumn: "login",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "experiment",
                columns: table => new
                {
                    experimentId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    extendedModelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_experiment", x => x.experimentId);
                    table.ForeignKey(
                        name: "FK_experiment_extendedModel_extendedModelId",
                        column: x => x.extendedModelId,
                        principalTable: "extendedModel",
                        principalColumn: "extendedModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "pattern",
                columns: table => new
                {
                    patternId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: true),
                    picture = table.Column<byte[]>(type: "bytea", nullable: true),
                    extendedModelId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_pattern", x => x.patternId);
                    table.ForeignKey(
                        name: "FK_pattern_extendedModel_extendedModelId",
                        column: x => x.extendedModelId,
                        principalTable: "extendedModel",
                        principalColumn: "extendedModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "recognisedPatterns",
                columns: table => new
                {
                    recognisedPatternsId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    patternId = table.Column<int>(type: "integer", nullable: false),
                    probability = table.Column<double>(type: "double precision", nullable: false),
                    PatternRecognitionExperimentexperimentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_recognisedPatterns", x => x.recognisedPatternsId);
                    table.ForeignKey(
                        name: "FK_recognisedPatterns_pattern_patternId",
                        column: x => x.patternId,
                        principalTable: "pattern",
                        principalColumn: "patternId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_recognisedPatterns_PatternRecognitionExperiment_PatternReco~",
                        column: x => x.PatternRecognitionExperimentexperimentId,
                        principalTable: "PatternRecognitionExperiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validationSet",
                columns: table => new
                {
                    validationSetId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    testedPattern = table.Column<byte[]>(type: "bytea", nullable: true),
                    truePatternId = table.Column<int>(type: "integer", nullable: false),
                    recognisedPatternId = table.Column<int>(type: "integer", nullable: false),
                    experimentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validationSet", x => x.validationSetId);
                    table.ForeignKey(
                        name: "FK_validationSet_ModelTrainingExperiment_experimentId",
                        column: x => x.experimentId,
                        principalTable: "ModelTrainingExperiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationSet_pattern_recognisedPatternId",
                        column: x => x.recognisedPatternId,
                        principalTable: "pattern",
                        principalColumn: "patternId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validationSet_pattern_truePatternId",
                        column: x => x.truePatternId,
                        principalTable: "pattern",
                        principalColumn: "patternId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_experiment_extendedModelId",
                table: "experiment",
                column: "extendedModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ExperimentExperimentList_experimentsexperimentId",
                table: "ExperimentExperimentList",
                column: "experimentsexperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_experimentList_userLogin_name",
                table: "experimentList",
                columns: new[] { "userLogin", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_extendedModel_userLogin_name",
                table: "extendedModel",
                columns: new[] { "userLogin", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ModelTrainingExperiment_extendedModelId1",
                table: "ModelTrainingExperiment",
                column: "extendedModelId1",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_pattern_extendedModelId",
                table: "pattern",
                column: "extendedModelId");

            migrationBuilder.CreateIndex(
                name: "IX_recognisedPatterns_patternId",
                table: "recognisedPatterns",
                column: "patternId");

            migrationBuilder.CreateIndex(
                name: "IX_recognisedPatterns_PatternRecognitionExperimentexperimentId",
                table: "recognisedPatterns",
                column: "PatternRecognitionExperimentexperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_user_email",
                table: "user",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_lastPatternRecognitionExperimentexperimentId",
                table: "user",
                column: "lastPatternRecognitionExperimentexperimentId");

            migrationBuilder.CreateIndex(
                name: "IX_validationSet_experimentId",
                table: "validationSet",
                column: "experimentId");

            migrationBuilder.CreateIndex(
                name: "IX_validationSet_recognisedPatternId",
                table: "validationSet",
                column: "recognisedPatternId");

            migrationBuilder.CreateIndex(
                name: "IX_validationSet_truePatternId",
                table: "validationSet",
                column: "truePatternId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExperimentExperimentList_experiment_experimentsexperimentId",
                table: "ExperimentExperimentList",
                column: "experimentsexperimentId",
                principalTable: "experiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExperimentExperimentList_experimentList_experimentListsexpe~",
                table: "ExperimentExperimentList",
                column: "experimentListsexperimentListId",
                principalTable: "experimentList",
                principalColumn: "experimentListId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTrainingExperiment_experiment_experimentId",
                table: "ModelTrainingExperiment",
                column: "experimentId",
                principalTable: "experiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelTrainingExperiment_extendedModel_extendedModelId1",
                table: "ModelTrainingExperiment",
                column: "extendedModelId1",
                principalTable: "extendedModel",
                principalColumn: "extendedModelId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatternRecognitionExperiment_experiment_experimentId",
                table: "PatternRecognitionExperiment",
                column: "experimentId",
                principalTable: "experiment",
                principalColumn: "experimentId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_experiment_extendedModel_extendedModelId",
                table: "experiment");

            migrationBuilder.DropTable(
                name: "ExperimentExperimentList");

            migrationBuilder.DropTable(
                name: "recognisedPatterns");

            migrationBuilder.DropTable(
                name: "validationSet");

            migrationBuilder.DropTable(
                name: "experimentList");

            migrationBuilder.DropTable(
                name: "ModelTrainingExperiment");

            migrationBuilder.DropTable(
                name: "pattern");

            migrationBuilder.DropTable(
                name: "extendedModel");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "PatternRecognitionExperiment");

            migrationBuilder.DropTable(
                name: "experiment");
        }
    }
}
