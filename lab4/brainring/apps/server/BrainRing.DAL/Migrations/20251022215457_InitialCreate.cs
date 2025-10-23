using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainRing.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AnswerEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SelectedOptionIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    HostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurrentQuestionId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Users_HostId",
                        column: x => x.HostId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSessionUsers",
                columns: table => new
                {
                    GameSessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessionUsers", x => new { x.GameSessionId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GameSessionUsers_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameSessionUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    GameSessionId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CorrectOptionIndex = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_GameSessions_GameSessionId",
                        column: x => x.GameSessionId,
                        principalTable: "GameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOptionEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    QuestionId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOptionEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOptionEntity_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnswerEntity_QuestionId_UserId",
                table: "AnswerEntity",
                columns: new[] { "QuestionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnswerEntity_UserId",
                table: "AnswerEntity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_CurrentQuestionId",
                table: "GameSessions",
                column: "CurrentQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_HostId",
                table: "GameSessions",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessionUsers_GameSessionId_UserId",
                table: "GameSessionUsers",
                columns: new[] { "GameSessionId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSessionUsers_UserId",
                table: "GameSessionUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptionEntity_QuestionId",
                table: "QuestionOptionEntity",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_GameSessionId",
                table: "Questions",
                column: "GameSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerEntity_Questions_QuestionId",
                table: "AnswerEntity",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSessions_Questions_CurrentQuestionId",
                table: "GameSessions",
                column: "CurrentQuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSessions_Questions_CurrentQuestionId",
                table: "GameSessions");

            migrationBuilder.DropTable(
                name: "AnswerEntity");

            migrationBuilder.DropTable(
                name: "GameSessionUsers");

            migrationBuilder.DropTable(
                name: "QuestionOptionEntity");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "GameSessions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
