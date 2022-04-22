using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.DisclaimerEngine.Postgres.Migrations
{
    public partial class ForeignKeyFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_disclaimers_Id",
                schema: "disclaimers",
                table: "questions");

            migrationBuilder.AddColumn<string>(
                name: "DisclaimerId",
                schema: "disclaimers",
                table: "questions",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_questions_DisclaimerId",
                schema: "disclaimers",
                table: "questions",
                column: "DisclaimerId");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_disclaimers_DisclaimerId",
                schema: "disclaimers",
                table: "questions",
                column: "DisclaimerId",
                principalSchema: "disclaimers",
                principalTable: "disclaimers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_disclaimers_DisclaimerId",
                schema: "disclaimers",
                table: "questions");

            migrationBuilder.DropIndex(
                name: "IX_questions_DisclaimerId",
                schema: "disclaimers",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "DisclaimerId",
                schema: "disclaimers",
                table: "questions");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_disclaimers_Id",
                schema: "disclaimers",
                table: "questions",
                column: "Id",
                principalSchema: "disclaimers",
                principalTable: "disclaimers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
