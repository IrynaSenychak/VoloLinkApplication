using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;

namespace VoloLinkApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<VoloLinkUser> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<VoloLinkUser> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();

            ViewBag.FirstName = user?.FirstName ?? "Guest";
            ViewBag.Role = roles.FirstOrDefault() ?? "No role";

            return View();

            //var user = await _userManager.GetUserAsync(User);
            //ViewBag.FirstName = user?.FirstName ?? "Guest"; // ѕередаЇмо ≥м'€ у ViewBag
            //return View();
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
