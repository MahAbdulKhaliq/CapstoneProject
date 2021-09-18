using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class PrimaryEquipmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrimaryEquipmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PrimaryEquipments
        public async Task<IActionResult> Index()
        {
            return View(await _context.PrimaryEquipment.ToListAsync());
        }

        // GET: PrimaryEquipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var primaryEquipment = await _context.PrimaryEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (primaryEquipment == null)
            {
                return NotFound();
            }

            return View(primaryEquipment);
        }

        // GET: PrimaryEquipments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PrimaryEquipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] PrimaryEquipment primaryEquipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(primaryEquipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(primaryEquipment);
        }

        // GET: PrimaryEquipments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var primaryEquipment = await _context.PrimaryEquipment.FindAsync(id);
            if (primaryEquipment == null)
            {
                return NotFound();
            }
            return View(primaryEquipment);
        }

        // POST: PrimaryEquipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] PrimaryEquipment primaryEquipment)
        {
            if (id != primaryEquipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(primaryEquipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PrimaryEquipmentExists(primaryEquipment.Id))
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
            return View(primaryEquipment);
        }

        // GET: PrimaryEquipments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var primaryEquipment = await _context.PrimaryEquipment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (primaryEquipment == null)
            {
                return NotFound();
            }

            return View(primaryEquipment);
        }

        // POST: PrimaryEquipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var primaryEquipment = await _context.PrimaryEquipment.FindAsync(id);
            _context.PrimaryEquipment.Remove(primaryEquipment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PrimaryEquipmentExists(int id)
        {
            return _context.PrimaryEquipment.Any(e => e.Id == id);
        }
    }
}
