using assignment3_b3w.DTO;
using assignment3_b3w.Hubs;
using assignment3_b3w.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace assignment3_b3w.Pages.Event
{
    [Authorize]
    public class EventFormModel : EventPageModel
    {

        public EventFormModel(EventManagementContext context, IHubContext<CRUDHub> hubContext) : base(context, hubContext)
        {
        }

        [BindProperty]
        public EventDto Event { get; set; }

        [BindProperty]
        public bool IsEditMode { get; set; }

        [BindProperty]
        public string? ErrorMessage { get; set; } = string.Empty;

        public SelectList CategoryList { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            CategoryList = new SelectList(await _context.EventCategories.ToListAsync(), nameof(EventCategory.CategoryId), nameof(EventCategory.CategoryName));

            if (!id.HasValue)
            {
                IsEditMode = false;
                Event = new EventDto();
            }
            else
            {
                IsEditMode = true;
                var eventModel = await _context.Events.FindAsync(id);

                if (eventModel == null)
                {
                    return RedirectToPage("/Event/Index", new { ErrorMessage = "Event not found or invalid access." });
                }
                Event = new EventDto
                {
                    EventId = eventModel.EventId,
                    Title = eventModel.Title,
                    Description = eventModel.Description,
                    StartTime = eventModel.StartTime,
                    EndTime = eventModel.EndTime,
                    Location = eventModel.Location,
                    CategoryId = eventModel.CategoryId
                };
            }
            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"Key: {state.Key}, Error: {error.ErrorMessage}");
                    }
                }

                CategoryList = new SelectList(await _context.EventCategories.ToListAsync(), nameof(EventCategory.CategoryId), nameof(EventCategory.CategoryName));
                return Page();
            }
            if (Event.StartTime >= Event.EndTime)
            {
                ErrorMessage = "End time must be later than start time.";
                CategoryList = new SelectList(await _context.EventCategories.ToListAsync(), nameof(EventCategory.CategoryId), nameof(EventCategory.CategoryName));
                return Page();
            }
            var eventModel = new Models.Event
            {
                Title = Event.Title,
                Description = Event.Description,
                StartTime = Event.StartTime,
                EndTime = Event.EndTime,
                Location = Event.Location,
                CategoryId = Event.CategoryId
            };

            if (IsEditMode)
            {
                eventModel.EventId = Event.EventId;
                _context.Attach(eventModel).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                _context.Events.Add(eventModel);
                await _context.SaveChangesAsync(); // Save changes to get the event id for the new event
                await _hubContext.Clients.All.SendAsync("EventAdded", new
                {
                    eventModel.EventId, // Send the event id to the client
                    eventModel.Title,
                    eventModel.Description,
                    eventModel.StartTime,
                    eventModel.EndTime,
                    eventModel.Location,
                    CategoryName = await _context.EventCategories.
                    Where(c => c.CategoryId == eventModel.CategoryId).
                    Select(c => c.CategoryName)
                    .FirstOrDefaultAsync(),
                    AttendeeCount = 0
                });
            }

            // Send update to all clients and update attendee count for the event on all clients
            await SendUpdateAsync(eventModel.EventId);
            await UpdateAttendeeCountAsync(eventModel.EventId);


            return RedirectToPage("/Event/Index");
        }
    }
}
