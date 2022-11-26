using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace PatternRecogniser.Migrations
{
    public partial class initDabase_fixErrors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    login = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    createDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    lastLog = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.login);
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
                    confusionMatrix = table.Column<int[]>(type: "integer[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModelTrainingExperiment", x => x.experimentId);
                    table.ForeignKey(
                        name: "FK_ModelTrainingExperiment_experiment_experimentId",
                        column: x => x.experimentId,
                        principalTable: "experiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_PatternRecognitionExperiment_experiment_experimentId",
                        column: x => x.experimentId,
                        principalTable: "experiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "authentication",
                columns: table => new
                {
                    userLogin = table.Column<string>(type: "text", nullable: false),
                    hashedToken = table.Column<string>(type: "text", nullable: true),
                    lastSeed = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authentication", x => x.userLogin);
                    table.ForeignKey(
                        name: "FK_authentication_user_userLogin",
                        column: x => x.userLogin,
                        principalTable: "user",
                        principalColumn: "login",
                        onDelete: ReferentialAction.Cascade);
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
                    distribution = table.Column<int>(type: "integer", nullable: false)
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
                name: "ExperimentExperimentList",
                columns: table => new
                {
                    experimentListsexperimentListId = table.Column<int>(type: "integer", nullable: false),
                    experimentsexperimentId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExperimentExperimentList", x => new { x.experimentListsexperimentListId, x.experimentsexperimentId });
                    table.ForeignKey(
                        name: "FK_ExperimentExperimentList_experiment_experimentsexperimentId",
                        column: x => x.experimentsexperimentId,
                        principalTable: "experiment",
                        principalColumn: "experimentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExperimentExperimentList_experimentList_experimentListsexpe~",
                        column: x => x.experimentListsexperimentListId,
                        principalTable: "experimentList",
                        principalColumn: "experimentListId",
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
                    PatternRecognitionExperimentexperimentId = table.Column<int>(type: "integer", nullable: true)
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
                        onDelete: ReferentialAction.Restrict);
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authentication");

            migrationBuilder.DropTable(
                name: "ExperimentExperimentList");

            migrationBuilder.DropTable(
                name: "recognisedPatterns");

            migrationBuilder.DropTable(
                name: "validationSet");

            migrationBuilder.DropTable(
                name: "experimentList");

            migrationBuilder.DropTable(
                name: "PatternRecognitionExperiment");

            migrationBuilder.DropTable(
                name: "ModelTrainingExperiment");

            migrationBuilder.DropTable(
                name: "pattern");

            migrationBuilder.DropTable(
                name: "experiment");

            migrationBuilder.DropTable(
                name: "extendedModel");

            migrationBuilder.DropTable(
                name: "user");
        }
    }
}
