using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class CreateProfiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Profile",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    WebsiteUserName = table.Column<string>(nullable: true),
                    AboutMe = table.Column<string>(nullable: true),
                    ShowHealthMetrics = table.Column<bool>(nullable: false),
                    Weight = table.Column<double>(nullable: false),
                    BodyFat = table.Column<double>(nullable: false),
                    Height = table.Column<double>(nullable: false),
                    ProfileImageResource = table.Column<string>(nullable: true),
                    MemberSince = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile", x => x.UserId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Profile");
        }
    }
}
