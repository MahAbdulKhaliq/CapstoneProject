using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public ExercisesController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        // GET: Exercises
        public async Task<IActionResult> Index(string searchString, string PrimaryEquipmentId, string MuscleGroupId)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            

            ViewBag.CurrentFilter = searchString;
            ViewBag.MuscleGroup = new SelectList(_context.MuscleGroup, "Id", "Name");
            ViewBag.PrimaryEquipment = new SelectList(_context.PrimaryEquipment, "Id", "Name");

            var tempQuery = from e in _context.Exercise
                            join m in _context.MuscleGroup on e.MuscleGroupId equals m.Id
                            join p in _context.PrimaryEquipment on e.PrimaryEquipmentId equals p.Id
                            select e;
            tempQuery = tempQuery.Include(m => m.MuscleGroup).Include(p => p.PrimaryEquipment);

            
            try
            {
                // Grab the user ID
                // Still throwing a NullReferenceException when user is logged out
                string userId = applicationUser.Id;
                // Query the FavouriteExercises db
                var favouriteTempQuery = from f in _context.FavouriteExercise
                                         where f.UserId == userId
                                         select f;

                // Create two filters of the main query: one that has favourite exercises,
                // and one that doesn't.
                var favouritedFilteredQuery = tempQuery.Where(e => favouriteTempQuery.Any(f => f.ExerciseId == e.Id));
                var unfavouritedFilteredQuery = tempQuery.Where(e => !favouriteTempQuery.Any(f => f.ExerciseId == e.Id));

                // Convert the 'favourited' option from 'false' to 'true' if it is found in the favourited filter query
                foreach (Exercise e in favouritedFilteredQuery)
                {
                    e.Favourited = true;
                }

                // Combine the queries
                tempQuery = favouritedFilteredQuery.Concat(unfavouritedFilteredQuery);
            }catch (NullReferenceException)
            {
                
            }
            

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

            // Order by favourited exercise
            query = query.OrderByDescending(e => e.Favourited).ToList();
            //Updated to retrieve MuscleGroup/PrimaryEquipment objects
            return View(query);
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            

            if (id == null)
            {
                return NotFound();
            }

            // First, grab exercise details
            var exercise = await _context.Exercise
                .Include(m => m.MuscleGroup)
                .Include(p => p.PrimaryEquipment)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            // Then, grab comments for that exercise
            var comments = from c in _context.Comment
                           where c.Discriminator == "Comment"
                           select c;

            comments = comments.Where(c => c.ExerciseId == id)
                                .OrderBy(c => c.Deleted)
                                .ThenByDescending(c => c.Date);

            // Grab replies for that exercise too
            var replies = from r in _context.Reply
                          where r.Discriminator == "Reply"
                          select r;

            replies = replies.Where(r => r.ExerciseId == id)
                                .OrderBy(r => r.Deleted)
                                .ThenByDescending(r => r.Date);


            try
            {
                // Grabs the User ID
                string userId = applicationUser.Id;


                // Checks to see if the comment belongs to the user,
                // or if the user is an admin. Admins can delete any comment.
                // Users can edit and delete their own comments.
                foreach (Comment comment in comments)
                {
                    if (comment.AuthorId == userId)
                    {
                        comment.Editable = true;
                        comment.Deletable = true;
                    }
                    if (User.IsInRole("Admin"))
                    {
                        comment.Deletable = true;
                    }
                }
                // Same operation with replies
                foreach (Reply reply in replies)
                {
                    if (reply.AuthorId == userId)
                    {
                        reply.Editable = true;
                        reply.Deletable = true;
                    }
                    if (User.IsInRole("Admin"))
                    {
                        reply.Deletable = true;
                    }
                }
            }catch(NullReferenceException)
            {
                // do nothing
            }
            
            // Finally, place that exercise's comments in the Comments IQueryable object
            // of the Exercise model.
            exercise.Comments = comments;
            exercise.Replies = replies;

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

                // Adds an ExerciseStats object for this newly added exercise
                ExerciseStats stats = new ExerciseStats
                {
                    ExerciseId = exercise.Id
                };
                _context.Add(stats);

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

            // Removes related ExerciseStat object
            var previousExerciseStat = await _context.ExerciseStats.FirstOrDefaultAsync(e => e.ExerciseId == id);

            _context.Remove(previousExerciseStat);

            // Remove related exercise from UserWorkoutExercises, WorkoutLogs, ViewedExercises
            var workoutLogQuery = from w in _context.WorkoutLog
                                  where w.ExerciseId == id
                                  select w;

            foreach (WorkoutLog log in workoutLogQuery)
            {
                _context.Remove(log);
            }

            var userWorkoutQuery = from w in _context.UserWorkoutExercise
                                  where w.ExerciseId == id
                                  select w;

            foreach (UserWorkoutExercise userExercise in userWorkoutQuery)
            {
                _context.Remove(userExercise);
            }

            var viewedExerciseQuery = from e in _context.ViewedExercise
                                   where e.ExerciseId == id
                                   select e;

            foreach (ViewedExercise userExercise in viewedExerciseQuery)
            {
                _context.Remove(userExercise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        

        private bool ExerciseExists(int id)
        {
            return _context.Exercise.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> _Fav(int exerciseId, string userId)
        {
            // Used to log errors and return as a JSON result
            List<string> errors = new List<string>(); 

            // DB processing: adds the new favourite exercise to the DB
            // for this user.

            FavouriteExercise newFavourite = new FavouriteExercise();

            newFavourite.ExerciseId = exerciseId;
            newFavourite.UserId = userId;
            newFavourite.DateFavourited = DateTime.Today;
            _context.FavouriteExercise.Add(newFavourite);
            await _context.SaveChangesAsync();

            return Json(errors);
        }

        [HttpPost]
        public async Task<IActionResult> _UnFav(int exerciseId, string userId)
        {
            // Used to log errors and return as a JSON result
            List<string> errors = new List<string>();

            // DB processing: removes the favourite exercise to the DB
            // for this user.

            var favExercise = await _context.FavouriteExercise
                .Where(m => m.UserId == userId)
                .FirstOrDefaultAsync(m => m.ExerciseId == exerciseId);
            try
            {
                _context.FavouriteExercise.Remove(favExercise);
                await _context.SaveChangesAsync();
            }
            catch (ArgumentNullException)
            {
                errors.Add("Null reference attempt.");
            }
            
            return Json(errors);
        }

        [HttpPost]
        public async Task<IActionResult> _Upvote(int exerciseId, string userId)
        {
            // Used to log errors and return as a JSON result
            List<string> errors = new List<string>();

            // DB processing: upvotes the object
            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.Id == exerciseId);

            exercise.PositiveRatings++;
            _context.Update(exercise);
            await _context.SaveChangesAsync();



            return Json(errors);
        }

        [HttpPost]
        public async Task<IActionResult> _Downvote(int exerciseId, string userId)
        {
            // Used to log errors and return as a JSON result
            List<string> errors = new List<string>();

            // DB processing: downvotes
            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.Id == exerciseId);

            exercise.NegativeRatings++;
            _context.Update(exercise);
            await _context.SaveChangesAsync();

            return Json(errors);
        }

        [HttpPost]
        public async Task<IActionResult> _PostComment([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Editable, Edited, DateEdited, Deletable, Deleted")] Comment comment)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: adding comments

            comment.AuthorId = userId;

            comment.Date = DateTime.Now;
            _context.Comment.Add(comment);
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = comment.ExerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> _EditComment([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Editable, Edited, DateEdited, Deletable, Deleted")] Comment comment, string editedText)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: editing comments

            var commentToBeEdited = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == comment.Id);

            commentToBeEdited.Content = comment.Content + "*";
            commentToBeEdited.Edited = true;
            commentToBeEdited.DateEdited = DateTime.Now;
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = comment.ExerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> _DeleteComment([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Editable, Edited, DateEdited, Deletable, Deleted")] Comment comment)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: 'deleting' comments

            var commentToBeDeleted = await _context.Comment
                .FirstOrDefaultAsync(m => m.Id == comment.Id);

            commentToBeDeleted.Deleted = true;
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = comment.ExerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> _PostReply([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Discriminator, ParentCommentId, Editable, Edited, DateEdited, Deletable, Deleted")] Reply reply)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: adding replies

            reply.AuthorId = userId;

            reply.Date = DateTime.Now;
            _context.Reply.Add(reply);
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = reply.ExerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> _EditReply([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Discriminator, ParentCommentId, Editable, Edited, DateEdited, Deletable, Deleted")] Reply reply)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: editing replies

            var replyToBeEdited = await _context.Reply
                .FirstOrDefaultAsync(m => m.Id == reply.Id);

            replyToBeEdited.Content = reply.Content + "*";
            replyToBeEdited.Edited = true;
            replyToBeEdited.DateEdited = DateTime.Now;
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = reply.ExerciseId });
        }

        [HttpPost]
        public async Task<IActionResult> _DeleteReply([Bind("Id, Author, AuthorId, ExerciseId, Content, Date, Discriminator, ParentCommentId, Editable, Edited, DateEdited, Deletable, Deleted")] Reply reply)
        {

            ApplicationUser applicationUser = await _userManager.GetUserAsync(User);
            string userId = applicationUser.Id;

            // DB processing: 'deleting' replies

            var replyToBeDeleted = await _context.Reply
                .FirstOrDefaultAsync(m => m.Id == reply.Id);

            replyToBeDeleted.Deleted = true;
            await _context.SaveChangesAsync();


            return RedirectToAction("Details", new { id = reply.ExerciseId });
        }

        
        [HttpPost]
        // Creates a 'ViewedExercise' object - used for admin stats
        public async Task<IActionResult> CreateViewedExercise(int exerciseId)
        {
            ViewedExercise viewedExercise = new ViewedExercise
            {
                ExerciseId = exerciseId,
                DateViewed = DateTime.Today
            };

            _context.Add(viewedExercise);
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }



    }
}
