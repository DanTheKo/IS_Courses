using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseService.Migrations
{
    /// <inheritdoc />
    public partial class quizzes_01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QuizResponseId",
                table: "QuestionAnswers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizResponses_QuizId",
                table: "QuizResponses",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionAnswers_QuizResponseId",
                table: "QuestionAnswers",
                column: "QuizResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_QuestionAnswerId",
                table: "Feedbacks",
                column: "QuestionAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_QuestionAnswers_QuestionAnswerId",
                table: "Feedbacks",
                column: "QuestionAnswerId",
                principalTable: "QuestionAnswers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionAnswers_QuizResponses_QuizResponseId",
                table: "QuestionAnswers",
                column: "QuizResponseId",
                principalTable: "QuizResponses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResponses_Quizzes_QuizId",
                table: "QuizResponses",
                column: "QuizId",
                principalTable: "Quizzes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_QuestionAnswers_QuestionAnswerId",
                table: "Feedbacks");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_Questions_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionAnswers_QuizResponses_QuizResponseId",
                table: "QuestionAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizResponses_Quizzes_QuizId",
                table: "QuizResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuizResponses_QuizId",
                table: "QuizResponses");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_QuestionId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_QuestionAnswers_QuizResponseId",
                table: "QuestionAnswers");

            migrationBuilder.DropIndex(
                name: "IX_Feedbacks_QuestionAnswerId",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "QuizResponseId",
                table: "QuestionAnswers");
        }
    }
}
