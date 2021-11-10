using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class ExerciseStats
    {
        
        [Key]
        public int Id { get; set; }
        [ForeignKey("Exercise")]
        [Display(Name = "Exercise ID")]
        public int ExerciseId { get; set; }
        [NotMapped]
        [Display(Name = "Exercise Name")]
        public string ExerciseName { get; set; }
        [NotMapped]
        public int Views { get; set; }
        [NotMapped]
        public int PositiveRatings { get; set; }
        [NotMapped]
        public int NegativeRatings { get; set; }
        [NotMapped]
        [Display(Name = "Number of Comments")]
        public int NumberOfComments { get; set; }
        [NotMapped]
        [Display(Name = "Included in Workouts")]
        public int IncludedInWorkouts { get; set; }
        [NotMapped]
        [Display(Name = "Placed in Log")]
        public int PlacedInLog { get; set; }

    }
}
