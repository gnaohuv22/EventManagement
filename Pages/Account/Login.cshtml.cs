using assignment3_b3w.Models;
using assignment3_b3w.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace assignment3_b3w.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IAuthentication _authentication;

        [BindProperty]
        public Credential Credential { get; set; } = new();

        [BindProperty]
        public string? ErrorMessage { get; set; } = string.Empty;

        public LoginModel(IAuthentication authentication)
        {
            _authentication = authentication;
        }

        public void OnGet(string? username = null)
        {
            if (!string.IsNullOrEmpty(username))
            {
                Credential.Username = username;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var result = await _authentication.Login(Credential.Username, Credential.Password);
            if (result != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.FullName),
                    new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                    new Claim(ClaimTypes.Role, result.Role)
                };
                //HttpContext.Session.SetString("userId", result.UserId.ToString());

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new(identity);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToPage("/Index");
            }
            else
            {
                ErrorMessage = "Email or password not correct or invalid.";
                return Page();
            }
        }
    }
}
