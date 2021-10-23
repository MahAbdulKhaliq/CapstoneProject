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
        public string UserId { get; set; }
        [Display(Name = "Username")]
        public string WebsiteUserName { get; set; }
        [Display(Name = "About Me")]
        public string AboutMe { get; set; }

        public bool ShowHealthMetrics { get; set; }
        public double Weight { get; set; }
        [Display(Name = "Body Fat Percentage")]
        public double BodyFat { get; set; }
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
