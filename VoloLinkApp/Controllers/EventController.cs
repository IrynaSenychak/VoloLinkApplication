using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VoloLinkApp.Areas.Identity.Data;
using VoloLinkApp.Models;

namespace VoloLinkApp.Controllers
{
    public class EventController : Controller
    {
        private readonly VoloLinkDbContext _context;
        private readonly UserManager<VoloLinkUser> _userManager;

        public EventController(VoloLinkDbContext context, UserManager<VoloLinkUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Get all events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Creator)
                .ToListAsync();
            return View(events);
        }
        public async Task<IActionResult> MarkEventComplete(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null) return NotFound();

            if (@event.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            @event.IsCompleted = true;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> MarkAttendance(int eventId, string userId, bool attended)
        {
            var @event = await _context.Events
                .Include(e => e.AttendedParticipants)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null) return NotFound();

            if (@event.CreatorId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Forbid();

            var participant = await _context.Users.FindAsync(userId);
            if (participant == null) return NotFound();

            if (attended)
                @event.AttendedParticipants.Add(participant);
            else
                @event.AttendedParticipants.Remove(participant);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = eventId });
        }
        //// Create new event
        //[HttpPost]
        //public async Task<IActionResult> Create(Event model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await _userManager.GetUserAsync(User);
        //        model.CreatorId = user.Id;

        //        _context.Events.Add(model);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> Details(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Creator)
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(currentUser);
            var isVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            var isAdmin = roles.Contains("Administrator");

           
            ViewBag.SideMenuData = currentUser;
            ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
            ViewBag.IsAdministrator = isAdmin;

            
            ViewBag.IsParticipating = currentUser != null && @event.Participants.Any(p => p.Id == currentUser.Id);
            ViewBag.IsCreator = currentUser != null && @event.CreatorId == currentUser.Id;
            ViewBag.IsVerifiedVolunteer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "VerifiedVolunteer");
           

            return View(@event);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> JoinEvent(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);

            if (@event.RequiresVerifiedVolunteer && !await _userManager.IsInRoleAsync(user, "VerifiedVolunteer"))
            {
                return BadRequest("This event requires verified volunteer status.");
            }

            if (!@event.Participants.Any(p => p.Id == user.Id))
            {
                @event.Participants.Add(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> WithdrawFromEvent(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var participant = @event.Participants.FirstOrDefault(p => p.Id == user.Id);

            if (participant != null)
            {
                @event.Participants.Remove(participant);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var @event = await _context.Events
                .FirstOrDefaultAsync(e => e.Id == id);

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(currentUser);
            var isVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            var isAdmin = roles.Contains("Administrator");

            // Set up side menu data
            ViewBag.SideMenuData = currentUser;
            ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
            ViewBag.IsAdministrator = isAdmin;


            if (@event == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the event
            
            if (@event.CreatorId != currentUser.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            return View(@event);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            // Check if the current user is the creator of the event
            var currentUser = await _userManager.GetUserAsync(User);
            if (@event.CreatorId != currentUser.Id && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    @event.Title = model.Title;
                    @event.Description = model.Description;
                    @event.Date = model.Date;
                    @event.Location = model.Location;
                    @event.RequiresVerifiedVolunteer = model.RequiresVerifiedVolunteer;

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(MyEvents));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(model);
        }
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveParticipant(int eventId, string userId)
        {
            var @event = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (@event == null)
            {
                return NotFound();
            }

            // Verify that the current user is the event creator
            var currentUser = await _userManager.GetUserAsync(User);
            if (@event.CreatorId != currentUser.Id && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            // Find the participant to remove
            var participantToRemove = @event.Participants.FirstOrDefault(p => p.Id == userId);
            if (participantToRemove != null)
            {
                @event.Participants.Remove(participantToRemove);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = eventId });
        }

        [Authorize]
        public async Task<IActionResult> MyEvents()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(currentUser);
            var isVerifiedVolunteer = roles.Contains("VerifiedVolunteer");
            var isAdmin = roles.Contains("Administrator");

            // Set up side menu data
            ViewBag.SideMenuData = currentUser;
            ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
            ViewBag.IsAdministrator = isAdmin;

            // Get events created by the user (only if verified or admin)
            ViewBag.CreatedEvents = isVerifiedVolunteer || isAdmin
                ? await _context.Events
                    .Include(e => e.Participants)
                    .Where(e => e.CreatorId == currentUser.Id)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync()
                : new List<Event>();

            // Get events where user is a participant
            ViewBag.ParticipatingEvents = await _context.Events
                .Include(e => e.Creator)
                .Include(e => e.Participants)
                .Where(e => e.Participants.Any(p => p.Id == currentUser.Id))
                .OrderBy(e => e.Date)
                .ToListAsync();

            return View();

        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            ViewBag.SideMenuData = user;
            var isVerifiedVolunteer = await _userManager.IsInRoleAsync(user, "VerifiedVolunteer");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            if (!isVerifiedVolunteer && !isAdmin)
            {
                return RedirectToAction("FindEvents", "User");
            }

            

            ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
            ViewBag.IsAdministrator = isAdmin;

            return View(new Event { Date = DateTime.Now.AddDays(1) });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var isVerifiedVolunteer = await _userManager.IsInRoleAsync(user, "VerifiedVolunteer");
            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");

            if (!isVerifiedVolunteer && !isAdmin)
            {
                return RedirectToAction("FindEvents", "User");
            }

            eventModel.CreatorId = user.Id;
            eventModel.Creator = user;
          

            // Debug ModelState errors
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                // You can check the errors in debug output
                System.Diagnostics.Debug.WriteLine($"ModelState Errors: {errors}");

                ViewBag.SideMenuData = user;
                ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
                ViewBag.IsAdministrator = isAdmin;
                ViewBag.ValidationErrors = errors; // Add this to see errors in the view
                return View(eventModel);
            }

            if (eventModel.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Дата події не може бути в минулому");
                ViewBag.SideMenuData = user;
                ViewBag.IsVerifiedVolunteer = isVerifiedVolunteer;
                ViewBag.IsAdministrator = isAdmin;
                return View(eventModel);
            }

            eventModel.CreatorId = user.Id;
            _context.Events.Add(eventModel);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Подію успішно створено";
            return RedirectToAction(nameof(MyEvents));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var @event = await _context.Events
                .Include(e => e.Participants)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

         
            var currentUser = await _userManager.GetUserAsync(User);
            if (@event.CreatorId != currentUser.Id && (!User.IsInRole("Administrator") || !User.IsInRole("VerifiedVolunteer")))
            {
                return Forbid();
            }

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            TempData["StatusMessage"] = "Подію успішно видалено";
            return RedirectToAction(nameof(MyEvents));
        }



        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }

}
