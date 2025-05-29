using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VoloLinkApp.Areas.Identity.Data;

namespace VoloLinkApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {

        private readonly UserManager<VoloLinkUser> _userManager;
        private readonly VoloLinkDbContext _context;

        public AdminController(UserManager<VoloLinkUser> userManager, VoloLinkDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }


        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> VerificationRequests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            ViewBag.SideMenuData = currentUser;
            ViewBag.IsAdministrator = true;
            ViewBag.IsVerifiedVolunteer = await _userManager.IsInRoleAsync(currentUser, "VerifiedVolunteer");

            var requests = await _context.VerificationRequests
                .Include(r => r.User)
                .Where(r => r.Status == "Pending")
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();

            return View(requests);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ProcessVerification(int requestId, bool approved, string response)
        {
            var request = await _context.VerificationRequests
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null) return NotFound();

            request.Status = approved ? "Approved" : "Rejected";
            request.AdminResponse = response;
            request.ProcessedDate = DateTime.UtcNow;

            if (approved)
            {
                await _userManager.AddToRoleAsync(request.User, "VerifiedVolunteer");
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(VerificationRequests));
        }
    }
}