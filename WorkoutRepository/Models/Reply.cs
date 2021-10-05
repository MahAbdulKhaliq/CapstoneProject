using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutRepository.Models
{
    public class Reply : Comment
    {
        public int CommentId { get; set; }
    }
}
