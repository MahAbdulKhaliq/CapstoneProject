using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class WorkoutLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public WorkoutLogsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // GET: WorkoutLogs
        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            // Grabs the user's workouts and places them in a SelectList for the View to use
            ViewBag.WorkoutList = new SelectList(_context.UserWorkout.Where(w => w.UserId == userId), "Id", "WorkoutName");
            ViewBag.LoadedDate = DateTime.Today.ToString("yyyy-MM-dd");

            var userQuery = from workoutLog in _context.WorkoutLog
                            select workoutLog;

            userQuery = userQuery.Where(w => w.UserId == userId).Where(w => w.DateFor == DateTime.Today);

            var finalQuery = await userQuery.ToListAsync();

            return View(finalQuery);
        }

        // POST: WorkoutLogs
        // Used to change the date for the currently viewed Index page
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(string dateFor)
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            // Grabs the user's workouts and places them in a SelectList for the View to use
            ViewBag.WorkoutList = new SelectList(_context.UserWorkout.Where(w => w.UserId == userId), "Id", "WorkoutName");
            ViewBag.LoadedDate = dateFor;

            var parsedDate = DateTime.Parse(dateFor);

            var userQuery = from workoutLog in _context.WorkoutLog
                            select workoutLog;

            userQuery = userQuery.Where(w => w.UserId == userId).Where(w => w.DateFor == parsedDate);

            var finalQuery = await userQuery.ToListAsync();

            return View(finalQuery);
        }

        [HttpPost]
        [AcceptVerbs("Post")]
        [Authorize]
        public async Task<IActionResult> SetWorkout(int workoutId, string dateFor)
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            // Grabs the user's workouts and places them in a SelectList for the View to use
            ViewBag.WorkoutList = new SelectList(_context.UserWorkout.Where(w => w.UserId == userId), "Id", "WorkoutName");
            ViewBag.LoadedDate = dateFor;

            // Parses the date as the incoming date is a String
            var parsedDate = DateTime.Parse(dateFor);

            // Queries the workout logs for ones under the specified date by this user
            var workoutLogQuery = from workoutLog in _context.WorkoutLog
                                  select workoutLog;

            workoutLogQuery = workoutLogQuery.Where(w => w.DateFor == parsedDate).Where(w => w.UserId == userId);

            // Clears out the current workout logs
            foreach (WorkoutLog log in workoutLogQuery)
            {
                _context.Remove(log);
            }

            await _context.SaveChangesAsync();


            // Next, queries the user's exercises in their workout
            var userWorkoutExerciseQuery = from exercise in _context.UserWorkoutExercise
                                           select exercise;

            userWorkoutExerciseQuery = userWorkoutExerciseQuery.Where(e => e.UserWorkoutId == workoutId);

            foreach (UserWorkoutExercise exercise in userWorkoutExerciseQuery)
            {
                for(int i = 1; i <= exercise.Sets ; i++)
                {
                    // Creates and adds a WorkoutLog object based off of each exercise, separately tracking
                    // each set with its own SetNumber
                    WorkoutLog log = new WorkoutLog
                    {
                        UserId = userId,
                        SetNumber = i,
                        Weight = 0,
                        Reps = 0,
                        ExerciseId = exercise.ExerciseId,
                        ExerciseName = exercise.ExerciseName,
                        DateFor = parsedDate
                    };
                    _context.Add(log);
                }
            }
            await _context.SaveChangesAsync();

            // Finally, returns to Index
            var userQuery = from workoutLog in _context.WorkoutLog
                            select workoutLog;

            userQuery = userQuery.Where(w => w.UserId == userId).Where(w => w.DateFor == parsedDate);

            var finalQuery = await userQuery.ToListAsync();

            // TO-DO: Return to previously listed date
            return RedirectToAction("Index", new { dateFor });
        }

        public async Task<IActionResult> _SaveWorkout(int workoutId, int weight, int reps)
        {
            
            // Finds the object to be edited
            var workoutLogToBeEdited = await _context.WorkoutLog
                .FirstOrDefaultAsync(w => w.Id == workoutId);

            // Updates it
            workoutLogToBeEdited.Weight = weight;
            workoutLogToBeEdited.Reps = reps;

            // Saves the changes to the context
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool WorkoutLogExists(int id)
        {
            return _context.WorkoutLog.Any(e => e.Id == id);
        }
    }
}
