using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class AddFavouriteExercises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercise",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_MuscleGroupId",
                table: "Exercise",
                column: "MuscleGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercise_PrimaryEquipmentId",
                table: "Exercise",
                column: "PrimaryEquipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_MuscleGroup_MuscleGroupId",
                table: "Exercise",
                column: "MuscleGroupId",
                principalTable: "MuscleGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exercise_PrimaryEquipment_PrimaryEquipmentId",
                table: "Exercise",
                column: "PrimaryEquipmentId",
                principalTable: "PrimaryEquipment",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_MuscleGroup_MuscleGroupId",
                table: "Exercise");

            migrationBuilder.DropForeignKey(
                name: "FK_Exercise_PrimaryEquipment_PrimaryEquipmentId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_MuscleGroupId",
                table: "Exercise");

            migrationBuilder.DropIndex(
                name: "IX_Exercise_PrimaryEquipmentId",
                table: "Exercise");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Exercise",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
