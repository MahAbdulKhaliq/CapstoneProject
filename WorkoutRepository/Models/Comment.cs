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

        // Grabs the discriminator for use later
        public string Discriminator { get; private set; }

        // WebsiteUserName attribute of comment creator
        public string Author { get; set; }
        // Identity Id attribute of comment creator
        [ForeignKey("AspNetUsers")]
        public string AuthorId { get; set; }
        // Related exercise this comment stems from
        [ForeignKey("Exercise")]
        public int ExerciseId { get; set; }
        // Comment content
        public string Content { get; set; }
        // Date comment was posted
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        // Used to see if this comment is editable by the user in ExercisesController
        [NotMapped]
        public bool Editable { get; set; } = false;
        // Flag to see if this comment has been edited before
        public bool Edited { get; set; } = false;
        // If edited, log the time
        public DateTime DateEdited { get; set; }
        // Used to see if this comment is deletable by users or admins in ExercisesController
        [NotMapped]
        public bool Deletable { get; set; } = false;
        // Flag to see if this comment has been deleted
        public bool Deleted { get; set; } = false;

    }
}
