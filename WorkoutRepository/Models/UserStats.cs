using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class UserStats
    {
        [Key]
        public int Id { get; set; }

        public string UserId { get; set; }
        [Display(Name = "IP Address")]
        public string IpAddress { get; set; }
        [Display(Name = "Date/Time")]
        public DateTime DateViewed { get; set; }
        [Display(Name = "Page Hit")]
        public string Url { get; set; }
    }
}
