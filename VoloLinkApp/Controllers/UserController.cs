using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;

namespace VoloLinkApp.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<VoloLinkUser> _userManager;
        private readonly VoloLinkDbContext _context;

        public UserController(UserManager<VoloLinkUser> userManager, VoloLinkDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Guest");
            }
            ViewBag.SideMenuData = user;
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRole = roles.FirstOrDefault() ?? "User";
            ViewBag.IsVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            ViewBag.IsAdministrator = roles.Contains("Administrator");
            return View(user);
        }

        [Authorize]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var viewModel = new UserSettingsViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            ViewBag.SideMenuData = user;
            ViewBag.IsVerifiedVolunteer = await _userManager.IsInRoleAsync(user, "VerifiedVolunteer");
            ViewBag.IsAdministrator = await _userManager.IsInRoleAsync(user, "Administrator");
            ViewBag.HasPendingRequest = await _context.VerificationRequests
                .AnyAsync(r => r.UserId == user.Id && r.Status == "Pending");

            if (!ViewBag.HasPendingRequest && !ViewBag.IsVerifiedVolunteer)
            {
                var lastRejectedRequest = await _context.VerificationRequests
                    .Where(r => r.UserId == user.Id && r.Status == "Rejected")
                    .OrderByDescending(r => r.ProcessedDate)
                    .FirstOrDefaultAsync();
                ViewBag.LastRejectedRequest = lastRejectedRequest;
            }
            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserSettingsViewModel model)
        {
            

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            // Check if username is already taken (but not by current user)
            if (currentUser.UserName != model.UserName)
            {
                var existingUser = await _userManager.FindByNameAsync(model.UserName);
                if (existingUser != null && existingUser.Id != currentUser.Id)
                {
                    ModelState.AddModelError("UserName", "Це ім'я користувача вже зайняте");
                    ViewBag.SideMenuData = currentUser;
                    ViewBag.IsVerifiedVolunteer = await _userManager.IsInRoleAsync(currentUser, "VerifiedVolunteer");
                    ViewBag.IsAdministrator = await _userManager.IsInRoleAsync(currentUser, "Administrator");
                    ViewBag.HasPendingRequest = await _context.VerificationRequests
                        .AnyAsync(r => r.UserId == currentUser.Id && r.Status == "Pending");
                    return View("Settings", model);
                }
            }

            try
            {
                // Update user properties
                currentUser.UserName = model.UserName;
                currentUser.FirstName = model.FirstName;
                currentUser.LastName = model.LastName;

                // Try to update the user
                var result = await _userManager.UpdateAsync(currentUser);

                if (result.Succeeded)
                {
                    TempData["StatusMessage"] = "Профіль успішно оновлено";
                    return RedirectToAction(nameof(Settings));
                }

                // If update failed, add errors to ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                ModelState.AddModelError("", "Виникла помилка при оновленні профілю");
            }

            // If we got here, something failed
            ViewBag.SideMenuData = currentUser;
            ViewBag.IsVerifiedVolunteer = await _userManager.IsInRoleAsync(currentUser, "VerifiedVolunteer");
            ViewBag.IsAdministrator = await _userManager.IsInRoleAsync(currentUser, "Administrator");
            ViewBag.HasPendingRequest = await _context.VerificationRequests
                .AnyAsync(r => r.UserId == currentUser.Id && r.Status == "Pending");

            return View("Settings", model);
        }

        [HttpPost]
        [Authorize]  // Add this attribute
        [ValidateAntiForgeryToken]  // Add this for security
        public async Task<IActionResult> RequestVerification(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                TempData["ErrorMessage"] = "Будь ласка, додайте повідомлення до вашого запиту";
                return RedirectToAction(nameof(Settings));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Check if user already has a pending request
            var existingRequest = await _context.VerificationRequests
                .AnyAsync(r => r.UserId == user.Id && r.Status == "Pending");

            if (existingRequest)
            {
                TempData["ErrorMessage"] = "У вас вже є активний запит на верифікацію";
                return RedirectToAction(nameof(Settings));
            }

            var request = new VerificationRequest
            {
                UserId = user.Id,
                RequestDate = DateTime.UtcNow,
                Status = "Pending",
                Message = message
            };

            _context.VerificationRequests.Add(request);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Заявку на верифікацію подано";
            return RedirectToAction(nameof(Settings));
        }

        public async Task<IActionResult> FindEvents(string searchName, string location, DateTime? fromDate, DateTime? toDate)
        {
           

           

            var user = await _userManager.GetUserAsync(User);
            var roles = user != null ? await _userManager.GetRolesAsync(user) : new List<string>();
            var isVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            var isAdmin = roles.Contains("Administrator");

            // Set up side menu data
            ViewBag.SideMenuData = user;
            ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
            ViewBag.IsAdministrator = isAdmin;
            var query = _context.Events
                .Include(e => e.Creator)
                .Where(e => e.Date >= DateTime.Now)
                .Where(e => !e.RequiresVerifiedVolunteer ||
                           (e.RequiresVerifiedVolunteer && (isVerifiedVolunteer || isAdmin)));

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(e => e.Title.Contains(searchName));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(e => e.Location.Contains(location));
            }

            if (fromDate.HasValue)
            {
                query = query.Where(e => e.Date >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(e => e.Date <= toDate.Value);
            }

            var events = await query.OrderBy(e => e.Date).ToListAsync();

            ViewBag.SearchName = searchName;
            ViewBag.Location = location;
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd");

            return View(events);
        }
        [AllowAnonymous]
        public IActionResult Guest()
        {
            return View("Profile", null);
        }
    }
}