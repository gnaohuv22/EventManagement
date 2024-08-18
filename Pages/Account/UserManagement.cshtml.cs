using assignment3_b3w.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace assignment3_b3w.Pages.Account
{
    [Authorize]
    public class UserModel : PageModel
    {
        private readonly EventManagementContext _context;

        public UserModel(EventManagementContext context)
        {
            _context = context;
        }

        public List<User> Users { get; set; }

        [BindProperty]
        public int UserId { get; set; }

        [BindProperty]
        public string NewRole { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var role = User.FindFirst(ClaimTypes.Role).Value;
            if (role == "Admin")
            {
                Users = await _context.Users.ToListAsync();
                return Page();
            }
            else
            {
                return RedirectToPage("/Account/AccessDenied");
            }
        }

        public async Task<IActionResult> OnPostChangeRoleAsync(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            user.Role = newRole;
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userEvents = await _context.Attendees.Where(a => a.UserId == userId).ToListAsync();
            _context.Attendees.RemoveRange(userEvents);
            _context.Users.Remove(user);

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
