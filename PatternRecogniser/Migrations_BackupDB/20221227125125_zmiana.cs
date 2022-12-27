using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations_BackupDB
{
    public partial class zmiana : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "num_classes",
                table: "extendedModel",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "num_classes",
                table: "extendedModel");
        }
    }
}
