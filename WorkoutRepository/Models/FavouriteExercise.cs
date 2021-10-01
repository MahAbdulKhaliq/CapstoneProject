using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class FavouriteExercise
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        [ForeignKey("AspNetUsers")]
        public string UserId { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateFavourited { get; set; }
    }
}
