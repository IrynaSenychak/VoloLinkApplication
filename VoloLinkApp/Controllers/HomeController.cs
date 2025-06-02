using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;

namespace VoloLinkApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<VoloLinkUser> _userManager;
        private readonly VoloLinkDbContext _context; // Add this field


        public HomeController(ILogger<HomeController> logger, UserManager<VoloLinkUser> userManager, VoloLinkDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
            var isVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            var isAdmin = roles.Contains("Administrator");

            ViewBag.FirstName = user?.FirstName ?? "Guest";
            ViewBag.Role = roles.FirstOrDefault() ?? "No role";


            var upcomingEvents = await _context.Events
                 .Include(e => e.Creator)
                 .Where(e => e.Date >= DateTime.Now)
                 .Where(e => !e.RequiresVerifiedVolunteer ||
                            (e.RequiresVerifiedVolunteer && (isVerifiedVolunteer || isAdmin)))
                 .OrderBy(e => e.Date)
                 .Take(3)
                 .ToListAsync();

            return View(upcomingEvents);

         
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
