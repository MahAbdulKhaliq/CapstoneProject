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
    public class UserStatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserStatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        // GET: UserStats
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string? userId)
        {
            var query = from userStat in _context.UserStats
                        select userStat;

            ViewBag.UserList = new SelectList(_context.Users, "Id", "WebsiteUserName");
            ViewBag.SelectedUserId = '0';

            if (userId != null)
            {
                ViewBag.SelectedUserId = userId;
                // Finds the user object for logging username
                var userSelected = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == userId);
                ViewBag.SelectedUserName = userSelected.WebsiteUserName;
                query = query.Where(u => u.UserId == userId);
            }
            query = query.OrderByDescending(u => u.DateViewed);

            var finalQuery = await query.ToListAsync();

            return View(finalQuery);
        }


        private bool UserStatsExists(int id)
        {
            return _context.UserStats.Any(e => e.Id == id);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUserStats(string userId, string url)
        {
            // Retrieve client IP address through HttpContext.Connection
            string clientIPAddr = HttpContext.Connection.RemoteIpAddress?.ToString();

            UserStats stats = new UserStats
            {
                UserId = userId,
                DateViewed = DateTime.Now,
                IpAddress = clientIPAddr,
                Url = url
            };

            _context.Add(stats);
            await _context.SaveChangesAsync();

            return new EmptyResult();
        }
    }
}
