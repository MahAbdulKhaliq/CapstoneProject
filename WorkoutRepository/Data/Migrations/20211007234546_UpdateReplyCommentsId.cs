using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class UpdateReplyCommentsId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "ParentCommentId",
                table: "Comment",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentCommentId",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "Comment",
                type: "int",
                nullable: true);
        }
    }
}
