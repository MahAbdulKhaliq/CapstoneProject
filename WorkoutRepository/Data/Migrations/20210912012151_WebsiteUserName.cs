using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class WebsiteUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebsiteUserName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WebsiteUserName",
                table: "AspNetUsers");
        }
    }
}
