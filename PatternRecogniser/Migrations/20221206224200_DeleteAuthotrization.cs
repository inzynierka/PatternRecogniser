using Microsoft.EntityFrameworkCore.Migrations;

namespace PatternRecogniser.Migrations
{
    public partial class DeleteAuthotrization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "authentication");

            migrationBuilder.AddColumn<string>(
                name: "hashedPassword",
                table: "user",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hashedPassword",
                table: "user");

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
        }
    }
}
