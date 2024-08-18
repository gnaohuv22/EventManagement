using Microsoft.AspNetCore.SignalR;

namespace assignment3_b3w.Hubs
{
    public class CRUDHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task UpdateEventInfo(int eventId)
        {
            await Clients.All.SendAsync("UpdateEventInfo", eventId);
        }

        public async Task UpdateAttendeeCount(int eventId, int count)
        {
            await Clients.All.SendAsync("UpdateAttendeeCount", eventId, count);
        }
    }
}
