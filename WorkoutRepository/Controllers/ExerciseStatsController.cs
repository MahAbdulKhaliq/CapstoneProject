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
        public async Task<IActionResult> Index(string? timeframe)
        {
            if (timeframe == null)
            {
                timeframe = "AllTime";
            }

            int daysTimeFrame = 0;

            switch (timeframe)
            {
                case "AllTime":
                    ViewBag.TimeFrameText = "All Time";
                    break;
                case "Today":
                    ViewBag.TimeFrameText = "Today";
                    daysTimeFrame = -1;
                    break;
                case "ThisWeek":
                    ViewBag.TimeFrameText = "This Week";
                    daysTimeFrame = -7;
                    break;
                case "ThisMonth":
                    ViewBag.TimeFrameText = "This Month";
                    daysTimeFrame = -30;
                    break;
                case "ThisYear":
                    ViewBag.TimeFrameText = "This Year";
                    daysTimeFrame = -365;
                    break;
                default:
                    break;
            }

            var statsQuery = from e in _context.ExerciseStats
                             select e;

            var exerciseQuery = from e in _context.Exercise
                                select e;

            foreach(ExerciseStats stats in statsQuery)
            {
                var relatedExercise = await _context.Exercise.FirstOrDefaultAsync(e => e.Id == stats.ExerciseId);

                var commentsQuery = from c in _context.Comment
                                    where c.ExerciseId == stats.ExerciseId
                                    where c.Deleted == false
                                    select c;

                var viewsQuery = from v in _context.ViewedExercise
                                 where v.ExerciseId == stats.ExerciseId
                                 where v.Discriminator == "ViewedExercise"
                                 select v;

                var placedInLogQuery = from v in _context.ViewedExercise
                                 where v.ExerciseId == stats.ExerciseId
                                 where v.Discriminator == "PlacedInLog"
                                 select v;

                var includedInWorkoutQuery = from v in _context.ViewedExercise
                                 where v.ExerciseId == stats.ExerciseId
                                 where v.Discriminator == "IncludedInWorkout"
                                 select v;

                if (timeframe != "AllTime")
                {
                    commentsQuery = commentsQuery.Where(c => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), c.Date) <= 0);
                    viewsQuery = viewsQuery.Where(v => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), v.DateViewed) <= 0);
                    placedInLogQuery = placedInLogQuery.Where(p => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), p.DateViewed) <= 0);
                    includedInWorkoutQuery = includedInWorkoutQuery.Where(i => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), i.DateViewed) <= 0);
                }

                stats.Views = viewsQuery.Count();
                stats.ExerciseName = relatedExercise.Name;
                stats.PositiveRatings = relatedExercise.PositiveRatings;
                stats.NegativeRatings = relatedExercise.NegativeRatings;
                stats.NumberOfComments = commentsQuery.Count();
                stats.PlacedInLog = placedInLogQuery.Count();
                stats.IncludedInWorkouts = includedInWorkoutQuery.Count();
            }

            var finalQuery = await statsQuery.ToListAsync();

            finalQuery = finalQuery.OrderByDescending(e => e.Views).ToList();
            

            return View(finalQuery);
        }
    }
}
