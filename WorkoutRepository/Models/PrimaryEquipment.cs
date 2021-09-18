using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class PrimaryEquipment
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
