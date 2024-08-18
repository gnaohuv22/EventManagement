using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using assignment3_b3w.Hubs;
using assignment3_b3w.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public abstract class EventPageModel : PageModel
{
    protected readonly EventManagementContext _context;
    protected readonly IHubContext<CRUDHub> _hubContext;

    public EventPageModel(EventManagementContext context, IHubContext<CRUDHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    protected int AttendeeCount { get; set; }
    protected bool isEnrolled { get; set; }
    protected async Task SendUpdateAsync(int eventId)
    {
        var userRole = User.FindFirstValue(ClaimTypes.Role);
        var updatedEvent = await _context.Events
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (updatedEvent != null)
        {
            var eventData = new
            {
                eventId = updatedEvent.EventId,
                title = updatedEvent.Title,
                description = updatedEvent.Description,
                startTime = updatedEvent.StartTime,
                endTime = updatedEvent.EndTime,
                location = updatedEvent.Location,
                category = updatedEvent.Category.CategoryName,
                categoryId = updatedEvent.CategoryId,
            };

            var attendeeGroups = await _context.Attendees
                .Where(a => a.EventId == eventId)
                .GroupBy(a => a.EventId)
                .Select(g => new { EventId = g.Key, Count = g.Count() })
                .ToListAsync();

            AttendeeCount = attendeeGroups.Count();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            isEnrolled = await _context.Attendees.AnyAsync(a => a.EventId == eventId && a.UserId == int.Parse(userId));

            await _hubContext.Clients.All.SendAsync("UpdateEventInfo", eventId, eventData, AttendeeCount, isEnrolled, userRole);
        }
    }

    protected async Task UpdateAttendeeCountAsync(int eventId)
    {
        var count = await _context.Attendees.CountAsync(a => a.EventId == eventId);
        await _hubContext.Clients.All.SendAsync("UpdateAttendeeCount", eventId, count);
    }
}