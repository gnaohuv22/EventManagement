using assignment3_b3w.DTO;
using assignment3_b3w.Models;
using assignment3_b3w.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using System.Text;

namespace assignment3_b3w.Pages.Event
{
    [Authorize]
    public class ReportModel : PageModel
    {
        private readonly ReportService _reportService;
        private readonly IUserServices _userService;

        public ReportModel(ReportService reportService, IUserServices userService)
        {
            _reportService = reportService;
            _userService = userService;
        }

        public int TotalEventCount { get; set; }
        public int TotalAttendeeCount { get; set; }
        public List<EventParticipationTrend> ParticipationTrends { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Admin")
            {
                return RedirectToPage("/Account/AccessDenied");
            }

            TotalEventCount = await _reportService.GetTotalEventCountAsync();
            TotalAttendeeCount = await _reportService.GetTotalAttendeeCountAsync();
            ParticipationTrends = await _reportService.GetParticipationTrendsAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostExportToCsvAsync()
        {
            var trends = await _reportService.GetParticipationTrendsAsync();
            var csv = new StringBuilder();
            csv.AppendLine("Date,Event Count,Attendee Count");

            foreach (var trend in trends)
            {
                csv.AppendLine($"{trend.Date:yyyy-MM-dd},{trend.EventCount},{trend.AttendeeCount}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", "ParticipationTrends.csv");
        }
    }

}
