using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class ViewedExercise
    {
        [Key]
        public int Id { get; set; }

        // Grabs the discriminator for use later
        public string Discriminator { get; private set; }

        public int ExerciseId { get; set; }

        public DateTime DateViewed { get; set; }
    }
}
