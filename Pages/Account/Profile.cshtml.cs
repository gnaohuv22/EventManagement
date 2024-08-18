using assignment3_b3w.Models;
using assignment3_b3w.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace assignment3_b3w.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly EventManagementContext _context;

        public ProfileModel(EventManagementContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string? ErrorMessage { get; set; } = string.Empty;
        [BindProperty]
        public int UserId { get; set; }

        [BindProperty]
        [Display(Name = "Username")]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [BindProperty]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [BindProperty]
        [Display(Name = "Full Name")]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [BindProperty]
        public string Role { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == "0")
            {
                // Handle default admin account
                UserId = 0;
                Username = "admin";
                Email = "admin@example.com";
                FullName = "Administrator";
                Role = "Admin";
                return Page();
            }
            if (userId == null)
            {
                return RedirectToPage("./Account/AccessDenied");
            }
            var user = await _context.Users.FindAsync(int.Parse(userId));

            if (user == null)
            {
                return NotFound();
            }
            UserId = user.UserId;
            Username = user.Username;
            Email = user.Email;
            FullName = user.FullName;
            Role = user.Role;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Username == Username && u.UserId != UserId);
            if (existingUser != null)
            {
                ModelState.AddModelError("Username", "Username is already taken.");
                return Page();
            }

            User user = _context.Users.FirstOrDefault(u => u.UserId == UserId);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = Username;
            user.Email = Email;
            user.FullName = FullName;

            await _context.SaveChangesAsync();
            ErrorMessage = "Saved successfully.";
            return Page();
        }
    }
}
