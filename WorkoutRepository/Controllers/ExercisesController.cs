using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExercisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exercises
        public async Task<IActionResult> Index(string searchString, string primaryEquipment, string muscleGroup)
        {
            ViewBag.CurrentFilter = searchString;
            ViewBag.MuscleGroup = new SelectList(_context.MuscleGroup, "Id", "Name");
            ViewBag.PrimaryEquipment = new SelectList(_context.PrimaryEquipment, "Id", "Name");

            var tempQuery = from e in _context.Exercise
                            join m in _context.MuscleGroup on e.MuscleGroupId equals m.Id
                            join p in _context.PrimaryEquipment on e.PrimaryEquipmentId equals p.Id
                            select e;
            tempQuery = tempQuery.Include(m => m.MuscleGroup).Include(p => p.PrimaryEquipment);

            //Return full list if searchString is empty
            //Or return a filtered list if searchString is not empty
            if (!string.IsNullOrEmpty(searchString))
            {
                tempQuery = tempQuery.Where(e => e.Name.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(primaryEquipment))
            {
                int primaryEquipmentId = Int32.Parse(primaryEquipment);
                tempQuery = tempQuery.Where(e => e.PrimaryEquipmentId.Equals(primaryEquipmentId));
            }
            if (!string.IsNullOrEmpty(muscleGroup))
            {
                int muscleGroupId = Int32.Parse(muscleGroup);
                tempQuery = tempQuery.Where(e => e.MuscleGroupId.Equals(muscleGroupId));
            }

            var query = await tempQuery.ToListAsync();
            //Updated to retrieve MuscleGroup/PrimaryEquipment objects
            return View(query);
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .Include(m => m.MuscleGroup)
                .Include(p => p.PrimaryEquipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        [Authorize(Roles = "Admin")]
        // GET: Exercises/Create
        public IActionResult Create()
        {
            ViewBag.MuscleGroupId = new SelectList(_context.MuscleGroup, "Id", "Name");
            ViewBag.PrimaryEquipmentId = new SelectList(_context.PrimaryEquipment, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MuscleGroupId,PrimaryEquipmentId,Name,ImageResource,EmbedLink,Description,PositiveRatings,NegativeRatings")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        [Authorize(Roles = "Admin")]
        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.MuscleGroupId = new SelectList(_context.MuscleGroup, "Id", "Name");
            ViewBag.PrimaryEquipmentId = new SelectList(_context.PrimaryEquipment, "Id", "Name");

            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        [Authorize(Roles = "Admin")]
        // POST: Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MuscleGroupId,PrimaryEquipmentId,Name,ImageResource,EmbedLink,Description,PositiveRatings,NegativeRatings")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id))
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
            return View(exercise);
        }

        [Authorize(Roles = "Admin")]
        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        [Authorize(Roles = "Admin")]
        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);
            _context.Exercise.Remove(exercise);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.Id == id);
        }
    }
}
