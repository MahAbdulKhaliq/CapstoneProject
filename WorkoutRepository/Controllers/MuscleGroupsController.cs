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
    public class MuscleGroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MuscleGroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        // GET: MuscleGroups
        public async Task<IActionResult> Index()
        {
            return View(await _context.MuscleGroup.ToListAsync());
        }

        [Authorize(Roles = "Admin")]
        // GET: MuscleGroups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var muscleGroup = await _context.MuscleGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (muscleGroup == null)
            {
                return NotFound();
            }

            return View(muscleGroup);
        }

        [Authorize(Roles = "Admin")]
        // GET: MuscleGroups/Create
        public IActionResult Create()
        {
            ViewBag.ErrorMessage = "";
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: MuscleGroups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] MuscleGroup muscleGroup)
        {
            if (ModelState.IsValid)
            {
                if (!MuscleGroupNameExists(muscleGroup.Name))
                {
                    _context.Add(muscleGroup);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ErrorMessage = "A muscle group with that name already exists.";
                    return View(muscleGroup);
                }
                
            }
            return View(muscleGroup);
        }

        [Authorize(Roles = "Admin")]
        // GET: MuscleGroups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var muscleGroup = await _context.MuscleGroup.FindAsync(id);
            if (muscleGroup == null)
            {
                return NotFound();
            }
            return View(muscleGroup);
        }

        [Authorize(Roles = "Admin")]
        // POST: MuscleGroups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] MuscleGroup muscleGroup)
        {
            if (id != muscleGroup.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(muscleGroup);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MuscleGroupExists(muscleGroup.Id))
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
            return View(muscleGroup);
        }

        [Authorize(Roles = "Admin")]
        // GET: MuscleGroups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var muscleGroup = await _context.MuscleGroup
                .FirstOrDefaultAsync(m => m.Id == id);
            if (muscleGroup == null)
            {
                return NotFound();
            }

            return View(muscleGroup);
        }

        [Authorize(Roles = "Admin")]
        // POST: MuscleGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (ExerciseExists(id))
            {
                TempData["ErrorMessage"] = "Exercise(s) with this muscle group already exists; please modify or delete exercise(s) before deleting this muscle group.";
                return RedirectToAction(nameof(Delete));
            }
            else
            {
                var muscleGroup = await _context.MuscleGroup.FindAsync(id);
                _context.MuscleGroup.Remove(muscleGroup);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
        }


        private bool MuscleGroupExists(int id)
        {
            return _context.MuscleGroup.Any(e => e.Id == id);
        }

        private bool MuscleGroupNameExists(string name)
        {
            return _context.MuscleGroup.Any(e => e.Name.ToLower() == name.ToLower());
        }

        private bool ExerciseExists(int id)
        {
           return _context.Exercise.Any(e => e.MuscleGroupId == id);
        }
    }
}
