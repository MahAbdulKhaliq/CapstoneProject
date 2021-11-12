using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkoutRepository.Data;
using WorkoutRepository.Models;

namespace WorkoutRepository.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfilesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        [Authorize]
        // GET: Profiles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var profile = await _context.Profile
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        [Authorize]
        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (_userManager.GetUserId(User) != id)
            {
                return Forbid();
            }

            var profile = await _context.Profile.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(string id, [Bind("UserId,WebsiteUserName,AboutMe,ShowHealthMetrics,Weight,BodyFat,Height,ProfileImageResource,ImageFile,MemberSince")] Profile profile)
        {
            if (id != profile.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Image uploader
                    // If the image is uploaded, add the image; otherwise, don't
                    if (profile.ImageFile != null)
                    {


                        // Deleting related image file
                        var oldImagePath = "";
                        try
                        {
                            oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, "images/profile/", profile.ProfileImageResource);
                        }


                        catch (ArgumentNullException) { }
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                        // Grabbing the uploaded image and save to wwwroot/images
                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Path.GetFileNameWithoutExtension(profile.ImageFile.FileName);
                        fileName = fileName.Replace(" ", "_");
                        string fileExtension = Path.GetExtension(profile.ImageFile.FileName);
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExtension;
                        if (fileName != "")
                        {
                            profile.ProfileImageResource = fileName;
                        }
                        string imagePath = Path.Combine(wwwRootPath + "/images/profile/", fileName);
                        using (var fileStream = new FileStream(imagePath, FileMode.Create))
                        {
                            await profile.ImageFile.CopyToAsync(fileStream);
                        }
                    }

                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = profile.UserId });
            }
            return View(profile);
        }

        private bool ProfileExists(string id)
        {
            return _context.Profile.Any(e => e.UserId == id);
        }
    }
}
