using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace WorkoutRepository.Models
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        // Used to find the appropriate Muscle Group name
        [ForeignKey("MuscleGroup")]
        [Display(Name = "Muscle Group ID")]
        public int MuscleGroupId { get; set; }
        [Display(Name = "Muscle Group")]
        public MuscleGroup MuscleGroup { get; set; }

        // Used to find the appropriate Primary Equipment name
        [ForeignKey("PrimaryEquipment")]
        [Display(Name = "Primary Equipment ID")]
        public int PrimaryEquipmentId { get; set; }
        [Display(Name = "Primary Equipment")]
        public PrimaryEquipment PrimaryEquipment { get; set; }

        // Name of the exercise
        [Required]
        public string Name { get; set; }

        // Image link reference
        [Display(Name = "Image")]
        public string ImageResource { get; set; }
        // Used for handling image uploads
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }
        // Used for handling video embed links
        [Display(Name = "Embed Link")]
        public string EmbedLink { get; set; }
        // Description of the exercise
        public string Description { get; set; }
        // Integer of positive ratings
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Positive Ratings")]
        // Integer of negative ratings
        public int PositiveRatings { get; set; } = 0;
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Negative Ratings")]
        public int NegativeRatings { get; set; } = 0;
        // Called to check if the current exercise is favourited in the ExercisesController
        [NotMapped]
        public bool Favourited { get; set; } = false;
        // Used for model purposes in appropriate Details.cshtml view
        [NotMapped]
        public Comment Comment { get; set; }
        // Used to hold comments for this exercise upon Details view load
        [NotMapped]
        public IQueryable<Comment> Comments { get; set; }
    }
}
