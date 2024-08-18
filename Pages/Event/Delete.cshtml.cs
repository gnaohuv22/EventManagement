using assignment3_b3w.Hubs;
using assignment3_b3w.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace assignment3_b3w.Pages.Event
{
    [Authorize]
    public class DeleteModel : EventPageModel
    {

        public DeleteModel(EventManagementContext context, IHubContext<CRUDHub> hubContext) : base(context, hubContext)
        { }

        [BindProperty]
        public Models.Event Event { get; set; }

        [BindProperty]
        public int attendeeCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.EventId == id);

            attendeeCount = await _context.Attendees
                .CountAsync(a => a.EventId == id);

            if (Event == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Event = await _context.Events.FindAsync(id);

            if (Event != null)
            {
                var eventId = Event.EventId;
                var attendees = await _context.Attendees
                    .Where(a => a.EventId == Event.EventId)
                    .ToListAsync();
                if (attendees.Count > 0)
                {
                    _context.Attendees.RemoveRange(attendees);
                }
                _context.Events.Remove(Event);
                await _context.SaveChangesAsync();

                await _hubContext.Clients.All.SendAsync("EventDeleted", eventId);

                // Send update to all clients
                await SendUpdateAsync(Event.EventId);

                //No need to update attendee count since the event is deleted
            }

            return RedirectToPage("./Index");
        }
    }
}
