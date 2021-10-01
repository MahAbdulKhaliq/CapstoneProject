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

        [ForeignKey("MuscleGroup")]
        [Display(Name = "Muscle Group ID")]
        public int MuscleGroupId { get; set; }
        [Display(Name = "Muscle Group")]
        public MuscleGroup MuscleGroup { get; set; }

        [ForeignKey("PrimaryEquipment")]
        [Display(Name = "Primary Equipment ID")]
        public int PrimaryEquipmentId { get; set; }
        [Display(Name = "Primary Equipment")]
        public PrimaryEquipment PrimaryEquipment { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Image")]
        public string ImageResource { get; set; }
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Embed Link")]
        public string EmbedLink { get; set; }

        public string Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Positive Ratings")]
        public int PositiveRatings { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Display(Name = "Negative Ratings")]
        public int NegativeRatings { get; set; }
        [NotMapped]
        public bool Favourited { get; set; } = false;
    }
}
