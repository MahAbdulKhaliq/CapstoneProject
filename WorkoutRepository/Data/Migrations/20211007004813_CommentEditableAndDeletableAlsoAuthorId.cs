using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class CommentEditableAndDeletableAlsoAuthorId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Exercise_ExerciseId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_ExerciseId",
                table: "Comment");

            migrationBuilder.AddColumn<string>(
                name: "AuthorId",
                table: "Comment",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateEdited",
                table: "Comment",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Comment",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Edited",
                table: "Comment",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "DateEdited",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Edited",
                table: "Comment");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_ExerciseId",
                table: "Comment",
                column: "ExerciseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Exercise_ExerciseId",
                table: "Comment",
                column: "ExerciseId",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
