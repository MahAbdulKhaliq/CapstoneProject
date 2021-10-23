using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class Profile
    {
        [Key]
        public string UserId { get; set; }
        [Display(Name = "Username")]
        public string WebsiteUserName { get; set; }
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }
        [Display(Name = "Show Health Metrics")]
        public bool ShowHealthMetrics { get; set; }
        [Range(0, 1000, ErrorMessage = "Value for {0} must be between {1}kg and {2}kg.")]
        [Display(Name = "Weight(kgs)")]
        public double Weight { get; set; }
        [Display(Name = "Body Fat Percentage")]
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1}% and {2}%.")]
        public double BodyFat { get; set; }
        [Display(Name = "Height(m)")]
        [Range(0, 3, ErrorMessage = "Value for {0} must be between {1}m and {2}m.")]
        public double Height { get; set; }

        // Image link reference
        [Display(Name = "Image")]
        public string ProfileImageResource { get; set; }
        // Used for handling image uploads
        [NotMapped]
        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }
        [Display(Name = "Member Since")]
        public DateTime MemberSince { get; set; }
    }
}
