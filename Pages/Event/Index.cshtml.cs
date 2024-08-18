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
    public class IndexModel : PageModel
    {

        public readonly EventManagementContext _context;

        public IndexModel(EventManagementContext context)
        {
            _context = context;
        }

        public Dictionary<int, int> AttendeeCounts { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, bool> UserEnrollmentStatus { get; set; } = new Dictionary<int, bool>();

        public IList<Models.Event> Events { get; set; }
        public SelectList CategoryList { get; set; }

        [BindProperty]
        public string UserRole { get; set; }

        [BindProperty]
        public string ErrorMessage { get; set; } = string.Empty;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryId { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var categories = await _context.EventCategories.ToListAsync();
            categories.Insert(0, new EventCategory { CategoryId = 0, CategoryName = "All" });

            CategoryList = new SelectList(categories, nameof(EventCategory.CategoryId), nameof(EventCategory.CategoryName));


            var query = _context.Events
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e => e.Title.Contains(SearchTerm) || e.Description.Contains(SearchTerm));
            }

            if (CategoryId.HasValue && CategoryId != 0)
            {
                query = query.Where(e => e.CategoryId == CategoryId.Value);
            }

            if (StartDate.HasValue)
            {
                query = query.Where(e => e.StartTime >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                query = query.Where(e => e.EndTime <= EndDate.Value.Date.AddDays(1).AddTicks(-1));
            }
            // Get all events
            Events = await query.Include(e => e.Category).ToListAsync();

            // Count attendees for each event
            var attendeeGroups = await _context.Attendees
                .GroupBy(a => a.EventId)
                .Select(g => new { EventId = g.Key, Count = g.Count() })
                .ToListAsync();

            AttendeeCounts = attendeeGroups.ToDictionary(g => g.EventId, g => g.Count);

            // Check if the current user is enrolled in each event
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            UserRole = User.FindFirstValue(ClaimTypes.Role);

            // Get all enrollments for the current user
            var userEnrollments = await _context.Attendees
                .Where(a => a.UserId == userId)
                .ToListAsync();

            // Create a dictionary with the event id as the key and a boolean indicating if the user is enrolled as the value
            UserEnrollmentStatus = userEnrollments.ToDictionary(a => a.EventId, a => userEnrollments.Any(e => e.EventId == a.EventId));

            return Page();
        }
    }
}
