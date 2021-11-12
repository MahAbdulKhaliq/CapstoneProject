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

            // Sets timeframe to AllTime if GET string is null
            if (timeframe == null)
            {
                timeframe = "AllTime";
            }

            int daysTimeFrame = 0; // Int used for comparing timeframe in later LINQ statements if AllTime is not selected

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

            // Queries ExerciseStats - has yet to put relevant data in model
            var statsQuery = from e in _context.ExerciseStats
                             select e;

            // Executes various counts for comments, views, placed in log, and included in workout operations
            foreach(ExerciseStats stats in statsQuery)
            {
                // Queries the related exercise by ID to grab its name
                var relatedExercise = await _context.Exercise.FirstOrDefaultAsync(e => e.Id == stats.ExerciseId);

                // All these other queries are for the relevant counts
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

                // If the timeframe is not set to 'All Time', also compare the dates to the relevant date selected.
                if (timeframe != "AllTime")
                {
                    commentsQuery = commentsQuery.Where(c => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), c.Date) <= 0);
                    viewsQuery = viewsQuery.Where(v => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), v.DateViewed) <= 0);
                    placedInLogQuery = placedInLogQuery.Where(p => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), p.DateViewed) <= 0);
                    includedInWorkoutQuery = includedInWorkoutQuery.Where(i => DateTime.Compare(DateTime.Today.AddDays(daysTimeFrame), i.DateViewed) <= 0);
                }

                // Add various attributes to the model
                stats.Views = viewsQuery.Count();
                stats.ExerciseName = relatedExercise.Name;
                stats.PositiveRatings = relatedExercise.PositiveRatings;
                stats.NegativeRatings = relatedExercise.NegativeRatings;
                stats.NumberOfComments = commentsQuery.Count();
                stats.PlacedInLog = placedInLogQuery.Count();
                stats.IncludedInWorkouts = includedInWorkoutQuery.Count();
            }

            // Convert the statsQuery to a list
            var finalQuery = await statsQuery.ToListAsync();

            // Order said list by views, descending
            finalQuery = finalQuery.OrderByDescending(e => e.Views).ToList();
            
            return View(finalQuery);
        }
    }
}
