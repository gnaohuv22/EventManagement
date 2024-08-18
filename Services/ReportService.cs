using assignment3_b3w.DTO;
using assignment3_b3w.Models;
using Microsoft.EntityFrameworkCore;

namespace assignment3_b3w.Services
{
    public class ReportService
    {
        private readonly EventManagementContext _context;

        public ReportService(EventManagementContext context)
        {
            _context = context;
        }

        public async Task<int> GetTotalEventCountAsync()
        {
            return await _context.Events.CountAsync();
        }

        public async Task<int> GetTotalAttendeeCountAsync()
        {
            return await _context.Attendees.CountAsync();
        }

        public async Task<List<EventParticipationTrend>> GetParticipationTrendsAsync()
        {
            // Bước 1: Lấy số lượng người tham dự cho mỗi sự kiện
            var attendeeCounts = await _context.Attendees
                .GroupBy(a => a.EventId)
                .Select(g => new { EventId = g.Key, AttendeeCount = g.Count() })
                .ToDictionaryAsync(x => x.EventId, x => x.AttendeeCount);

            // Bước 2: Lấy thông tin sự kiện và tính toán xu hướng
            var eventInfo = await _context.Events
                .Where(e => e.StartTime.HasValue)
                .GroupBy(e => e.StartTime.Value.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    EventCount = g.Count(),
                    EventIds = g.Select(e => e.EventId).ToList()
                })
                .ToListAsync();

            // Bước 3: Kết hợp thông tin
            var trends = eventInfo.Select(ei => new EventParticipationTrend
            {
                Date = ei.Date,
                EventCount = ei.EventCount,
                AttendeeCount = ei.EventIds.Sum(eventId =>
                    attendeeCounts.ContainsKey(eventId) ? attendeeCounts[eventId] : 0)
            }).ToList();

            return trends;
        }

    }

}
