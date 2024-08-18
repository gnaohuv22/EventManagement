using assignment3_b3w.Models;
using assignment3_b3w.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace assignment3_b3w.Pages.Account
{
    public class RegisterInput
    {
        [Required]
        [DataType(DataType.Text)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Role { get; set; }
    }
    public class RegisterModel : PageModel
    {
        private readonly IAuthentication _authentication;
        public RegisterModel(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        [BindProperty]
        public RegisterInput Register { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = new User
            {
                Username = Register.Username,
                Email = Register.Email,
                Password = Register.Password,
                FullName = Register.FullName,
                Role = Register.Role,
            };
            var result = await _authentication.Register(user);
            if (result)
            {
                return RedirectToPage("/Account/Login", new { username = Register.Username });
            }
            else
            {
                ErrorMessage = "Invalid registration attempt. Username already has been taken.";
                return Page();
            }
        }
    }
}
