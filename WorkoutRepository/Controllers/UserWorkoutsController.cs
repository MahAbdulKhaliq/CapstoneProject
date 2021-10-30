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
    public class UserWorkoutsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserWorkoutsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        [Authorize]
        // GET: UserWorkouts
        public async Task<IActionResult> Index()
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            var query = from uWorkout in _context.UserWorkout
                        select uWorkout;

            var userFilteredQuery = query.Where(uWorkout => uWorkout.UserId == userId);

            var finalQuery = await userFilteredQuery.ToListAsync();

            return View(finalQuery);
        }

        [Authorize]
        // GET: UserWorkouts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            // Grabs the exercise ID's and names and places them in a SelectList for the View to use
            ViewBag.ExerciseList = new SelectList(_context.Exercise, "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }

            // First, grab the user's workout
            var userWorkout = await _context.UserWorkout
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userWorkout == null)
            {
                return NotFound();
            }
            // If the workout does not belong to the user, return a forbidden
            // page.
            if (userWorkout.UserId != userId)
            {
                return Forbid();
            }
            // Then, grab the exercises in the user's workout
            var exercises = from e in _context.UserWorkoutExercise
                            select e;

            exercises = exercises.Where(e => e.UserWorkoutId == id);

            userWorkout.UserWorkoutExercises = exercises;

            return View(userWorkout);
        }

        [Authorize]
        // GET: UserWorkouts/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
        // POST: UserWorkouts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,WorkoutName,UserId")] UserWorkout userWorkout)
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;
            if (ModelState.IsValid)
            {
                // Adds the user ID to the currently added workout;
                // used to filter view by user ID, and add authorization
                userWorkout.UserId = userId;
                _context.Add(userWorkout);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userWorkout);
        }

        [Authorize]
        // GET: UserWorkouts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userWorkout = await _context.UserWorkout.FindAsync(id);
            if (userWorkout == null)
            {
                return NotFound();
            }
            return View(userWorkout);
        }

        [Authorize]
        // POST: UserWorkouts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,WorkoutName,UserId")] UserWorkout userWorkout)
        {
            if (id != userWorkout.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userWorkout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserWorkoutExists(userWorkout.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(userWorkout);
        }

        [Authorize]
        // GET: UserWorkouts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userWorkout = await _context.UserWorkout
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userWorkout == null)
            {
                return NotFound();
            }

            return View(userWorkout);
        }

        [Authorize]
        // POST: UserWorkouts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userWorkout = await _context.UserWorkout.FindAsync(id);
            _context.UserWorkout.Remove(userWorkout);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserWorkoutExists(int id)
        {
            return _context.UserWorkout.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> _AddExercise([Bind("Id, ExerciseId, ExerciseName, UserWorkoutId, Sets")]UserWorkoutExercise userWorkoutExercise)
        {
            // Grabs the user ID
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser?.Id;

            // Grabbing the related exercise to pull its name into the userWorkoutExercise
            var relatedExercise = await _context.Exercise
                .FirstOrDefaultAsync(e => e.Id == userWorkoutExercise.ExerciseId);

            userWorkoutExercise.ExerciseName = relatedExercise.Name;

            _context.UserWorkoutExercise.Add(userWorkoutExercise);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = userWorkoutExercise.UserWorkoutId});
        }
    }
}
