using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        public string Author { get; set; }

        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }

        public string Content { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
