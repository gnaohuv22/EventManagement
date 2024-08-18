using assignment3_b3w.Hubs;
using assignment3_b3w.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace assignment3_b3w.Pages.Event
{
    [Authorize]
    public class EnrollConfirmationModel : EventPageModel
    {

        public EnrollConfirmationModel(EventManagementContext context, IHubContext<CRUDHub> hubContext) : base(context, hubContext)
        { }

        [BindProperty]
        public Models.Event Event { get; set; }

        [BindProperty]
        public bool IsEnroll { get; set; }

        [BindProperty]
        public User user { get; set; }

        public async Task<IActionResult> OnGetAsync(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            Event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.EventId == eventId);

            if (Event == null)
            {
                return NotFound();
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId == 0)
            {
                user = new User
                {
                    Username = "Admin",
                    Email = "admin@example.com",
                    FullName = "Administrator",
                    Role = "Admin"
                };
            }
            else
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }

            if (user == null)
            {
                return NotFound();
            }

            var existingAttendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);

            IsEnroll = existingAttendee == null;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? eventId)
        {
            if (eventId == null)
            {
                return NotFound();
            }

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (userId == 0)
            {
                user = new User
                {
                    Username = "Admin",
                    Email = "admin@example.com",
                    FullName = "Administrator",
                    Role = "Admin"
                };
            }
            else
            {
                user = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }

            if (user == null)
            {
                return NotFound();
            }

            var existingAttendee = await _context.Attendees
                .FirstOrDefaultAsync(a => a.EventId == eventId && a.UserId == userId);

            if (existingAttendee == null)
            {
                var attendee = new Attendee
                {
                    EventId = (int)eventId,
                    UserId = userId,
                    Name = user.FullName,
                    Email = user.Email,
                    RegistrationTime = DateTime.Now
                };
                _context.Attendees.Add(attendee);
            }
            else
            {
                _context.Attendees.Remove(existingAttendee);
            }

            await _context.SaveChangesAsync();

            // Send update to all clients and update attendee count for the event on all clients
            await SendUpdateAsync((int)eventId);
            await UpdateAttendeeCountAsync((int)eventId);

            return RedirectToPage("/Event/Index");
        }
    }
}
