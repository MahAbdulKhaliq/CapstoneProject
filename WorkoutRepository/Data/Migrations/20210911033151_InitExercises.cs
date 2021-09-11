using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class InitExercises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Exercise",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MuscleGroupId = table.Column<int>(nullable: false),
                    PrimaryEquipmentId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    ImageResource = table.Column<string>(nullable: true),
                    EmbedLink = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    PositiveRatings = table.Column<int>(nullable: false),
                    NegativeRatings = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercise", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Exercise");
        }
    }
}
