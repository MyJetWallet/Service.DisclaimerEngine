using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Service.DisclaimerEngine.Postgres.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "disclaimers");

            migrationBuilder.CreateTable(
                name: "contexts",
                schema: "disclaimers",
                columns: table => new
                {
                    DisclaimerId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contexts", x => new { x.ClientId, x.DisclaimerId });
                });

            migrationBuilder.CreateTable(
                name: "disclaimers",
                schema: "disclaimers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    CreationTs = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TitleTemplateId = table.Column<string>(type: "text", nullable: true),
                    DescriptionTemplateId = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    ShowMarketingEmailQuestion = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_disclaimers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "answers",
                schema: "disclaimers",
                columns: table => new
                {
                    QuestionId = table.Column<string>(type: "text", nullable: false),
                    ClientId = table.Column<string>(type: "text", nullable: false),
                    DisclaimerId = table.Column<string>(type: "text", nullable: true),
                    Result = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_answers", x => new { x.ClientId, x.QuestionId });
                    table.ForeignKey(
                        name: "FK_answers_contexts_ClientId_DisclaimerId",
                        columns: x => new { x.ClientId, x.DisclaimerId },
                        principalSchema: "disclaimers",
                        principalTable: "contexts",
                        principalColumns: new[] { "ClientId", "DisclaimerId" });
                });

            migrationBuilder.CreateTable(
                name: "questions",
                schema: "disclaimers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    TextTemplateId = table.Column<string>(type: "text", nullable: true),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DefaultState = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_questions_disclaimers_Id",
                        column: x => x.Id,
                        principalSchema: "disclaimers",
                        principalTable: "disclaimers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_answers_ClientId_DisclaimerId",
                schema: "disclaimers",
                table: "answers",
                columns: new[] { "ClientId", "DisclaimerId" });

            migrationBuilder.CreateIndex(
                name: "IX_contexts_ClientId",
                schema: "disclaimers",
                table: "contexts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_contexts_DisclaimerId",
                schema: "disclaimers",
                table: "contexts",
                column: "DisclaimerId");

            migrationBuilder.CreateIndex(
                name: "IX_disclaimers_Type",
                schema: "disclaimers",
                table: "disclaimers",
                column: "Type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "answers",
                schema: "disclaimers");

            migrationBuilder.DropTable(
                name: "questions",
                schema: "disclaimers");

            migrationBuilder.DropTable(
                name: "contexts",
                schema: "disclaimers");

            migrationBuilder.DropTable(
                name: "disclaimers",
                schema: "disclaimers");
        }
    }
}
