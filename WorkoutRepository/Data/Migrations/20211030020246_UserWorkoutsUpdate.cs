using Microsoft.EntityFrameworkCore.Migrations;

namespace WorkoutRepository.Data.Migrations
{
    public partial class UserWorkoutsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserWorkoutExercise_Exercise_ExerciseId",
                table: "UserWorkoutExercise");

            migrationBuilder.DropIndex(
                name: "IX_UserWorkoutExercise_ExerciseId",
                table: "UserWorkoutExercise");

            migrationBuilder.AddColumn<string>(
                name: "ExerciseName",
                table: "UserWorkoutExercise",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExerciseName",
                table: "UserWorkoutExercise");

            migrationBuilder.CreateIndex(
                name: "IX_UserWorkoutExercise_ExerciseId",
                table: "UserWorkoutExercise",
                column: "ExerciseId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserWorkoutExercise_Exercise_ExerciseId",
                table: "UserWorkoutExercise",
                column: "ExerciseId",
                principalTable: "Exercise",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
