using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class UserWorkoutExercise
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Exercise")]
        [Display(Name = "Exercise ID")]
        public int ExerciseId { get; set; }

        public string ExerciseName { get; set; }

        [ForeignKey("UserWorkout")]
        [Display(Name = "User Workout ID")]
        public int UserWorkoutId { get; set; }

        public int Sets { get; set; }

    }
}
