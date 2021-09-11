using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Pages.Exercises
{
    public class IndexModel : PageModel
    {
        private readonly WorkoutRepository.Data.ApplicationDbContext _context;

        public IndexModel(WorkoutRepository.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Exercise> Exercise { get;set; }

        public async Task OnGetAsync()
        {
            Exercise = await _context.Exercise.ToListAsync();
        }
    }
}
