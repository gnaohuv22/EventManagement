using assignment3_b3w.Models;
using assignment3_b3w.Services;
using Microsoft.AspNetCore.Mvc;

namespace assignment3_b3w.Settings
{
    public interface IAuthentication
    {
        Task<User?> Login(string username, string password);
        Task<bool> Register(User user);
    }
    public class Authentication : IAuthentication
    {
        private readonly IUserServices _userServices;

        public Authentication(IUserServices userServices)
        {
            _userServices = userServices;
        }

        public async Task<User?> Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                return new User
                {
                    UserId = 0,
                    Username = "admin",
                    Email = "admin@example.com",
                    FullName = "Administrator",
                    Role = "Admin"
                };
            }


            var entry = await _userServices.GetUserByUsername(username);
            if (entry != null)
            {
                if (entry.Password.Equals(password))
                {
                    return entry;
                }
            }
            return null;
        }

        public async Task<bool> Register(User user)
        {
            var entry = await _userServices.GetUserByUsername(user.Username);
            if (entry == null)
            {
                await _userServices.AddOrEditUser(user);
                return true;
            }
            return false;
        }
    }
}
