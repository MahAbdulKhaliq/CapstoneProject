using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class UserWorkout
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Workout Name")]
        public string WorkoutName { get; set; }

        public string UserId { get; set; }

        [NotMapped]
        public UserWorkoutExercise UserWorkoutExercise { get; set; }

        [NotMapped]
        public IQueryable<UserWorkoutExercise> UserWorkoutExercises { get; set; }
    }
}
