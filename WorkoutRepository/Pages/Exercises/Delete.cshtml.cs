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
    public class DeleteModel : PageModel
    {
        private readonly WorkoutRepository.Data.ApplicationDbContext _context;

        public DeleteModel(WorkoutRepository.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Exercise Exercise { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Exercise = await _context.Exercise.FirstOrDefaultAsync(m => m.Id == id);

            if (Exercise == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Exercise = await _context.Exercise.FindAsync(id);

            if (Exercise != null)
            {
                _context.Exercise.Remove(Exercise);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
