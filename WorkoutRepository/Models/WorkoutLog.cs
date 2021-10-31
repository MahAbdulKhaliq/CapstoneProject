using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class WorkoutLog
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        public string ExerciseName { get; set; }

        public int SetNumber { get; set; }

        public double Weight { get; set; }

        public int Reps { get; set; }

        public DateTime DateFor { get; set; }

    }
}
