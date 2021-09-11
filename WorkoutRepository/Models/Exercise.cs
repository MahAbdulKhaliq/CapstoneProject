using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WorkoutRepository.Models
{
    public class Exercise
    {
        public int Id { get; set; }

        [Display(Name = "Muscle Group ID")]
        public int MuscleGroupId { get; set; }
        [Display(Name = "Primary Equipment ID")]
        public int PrimaryEquipmentId { get; set; }

        public string Name { get; set; }
        [Display(Name = "Image Link")]
        public string ImageResource { get; set; }
        [Display(Name = "Embed Link")]
        public string EmbedLink { get; set; }

        public string Description { get; set; }
        [Display(Name = "Positive Ratings")]
        public int PositiveRatings { get; set; }
        [Display(Name = "Negative Ratings")]
        public int NegativeRatings { get; set; }
    }
}
