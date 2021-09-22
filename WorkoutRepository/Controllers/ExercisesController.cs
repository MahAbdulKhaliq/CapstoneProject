﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ExercisesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Exercises
        public async Task<IActionResult> Index(string searchString, string PrimaryEquipmentId, string MuscleGroupId)
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
            if (!string.IsNullOrEmpty(PrimaryEquipmentId))
            {
                int primaryEquipmentId = Int32.Parse(PrimaryEquipmentId);
                tempQuery = tempQuery.Where(e => e.PrimaryEquipmentId.Equals(primaryEquipmentId));
            }
            if (!string.IsNullOrEmpty(MuscleGroupId))
            {
                int muscleGroupId = Int32.Parse(MuscleGroupId);
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
        public async Task<IActionResult> Create([Bind("Id,MuscleGroupId,PrimaryEquipmentId,Name,ImageFile,EmbedLink,Description,PositiveRatings,NegativeRatings")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                // If the image is uploaded, add the image; otherwise, don't
                if (exercise.ImageFile != null)
                {
                    // Grabbing the uploaded image and save to wwwroot/images
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(exercise.ImageFile.FileName);
                    string fileExtension = Path.GetExtension(exercise.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExtension;
                    exercise.ImageResource = fileName;
                    string imagePath = Path.Combine(wwwRootPath + "/images", fileName);
                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await exercise.ImageFile.CopyToAsync(fileStream);
                    }
                }
                
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,MuscleGroupId,PrimaryEquipmentId,Name,ImageResource,ImageFile,EmbedLink,Description,PositiveRatings,NegativeRatings")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // If the image is uploaded, add the image; otherwise, don't
                    if (exercise.ImageFile != null)
                    {
                        
                        
                        // Deleting related image file
                        var oldImagePath = "";
                        try
                        {
                            oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", exercise.ImageResource);
                        }


                        catch (ArgumentNullException) { }
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        
                        // Grabbing the uploaded image and save to wwwroot/images
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(exercise.ImageFile.FileName);
                        string fileExtension = Path.GetExtension(exercise.ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExtension;
                        if (fileName != "")
                        {
                            exercise.ImageResource = fileName;
                        }                       
                        string imagePath = Path.Combine(wwwRootPath + "/images", fileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await exercise.ImageFile.CopyToAsync(fileStream);
                        }
                    }

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
        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exercise = await _context.Exercise.FindAsync(id);

            // Deleting related image file
            var imagePath = "";
            try
            {
                imagePath = Path.Combine(_hostEnvironment.WebRootPath, "images", exercise.ImageResource);
            }
            catch (ArgumentNullException) { }       
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }

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
