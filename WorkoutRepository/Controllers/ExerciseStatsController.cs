using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class ExerciseStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExerciseStats
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var baseQuery = await _context.ExerciseStats.ToListAsync();

            return View(baseQuery);
        }

       

        private bool ExerciseStatsExists(int id)
        {
            return _context.ExerciseStats.Any(e => e.Id == id);
        }
    }
}
