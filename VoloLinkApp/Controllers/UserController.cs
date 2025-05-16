using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using VoloLinkApp.Areas.Identity.Data;

namespace VoloLinkApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<VoloLinkUser> _userManager;

        public UserController(UserManager<VoloLinkUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Guest");
            }

            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = roles.FirstOrDefault() ?? "User";
            ViewBag.IsVerifiedVolunteer = roles.Contains("VerifiedVolunteer");

            return View(user);
        }

        [AllowAnonymous]
        public IActionResult Guest()
        {
            return View("Profile", null);
        }
    }
}