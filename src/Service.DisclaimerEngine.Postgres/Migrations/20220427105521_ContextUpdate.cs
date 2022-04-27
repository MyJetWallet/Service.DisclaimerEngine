using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.DisclaimerEngine.Postgres.Migrations
{
    public partial class ContextUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReplacedWithNewerDisclaimer",
                schema: "disclaimers",
                table: "contexts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplacedWithNewerDisclaimer",
                schema: "disclaimers",
                table: "contexts");
        }
    }
}
