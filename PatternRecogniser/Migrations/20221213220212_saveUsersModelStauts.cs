using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class saveUsersModelStauts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lastCheckModel",
                table: "user",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "lastModelStatus",
                table: "user",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lastCheckModel",
                table: "user");

            migrationBuilder.DropColumn(
                name: "lastModelStatus",
                table: "user");
        }
    }
}
