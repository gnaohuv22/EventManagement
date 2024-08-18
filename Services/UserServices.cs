using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using assignment3_b3w.Models;

namespace assignment3_b3w.Services
{

    public interface IUserServices
    {
        Task<List<User>> GetAll();
        Task<List<User>> GetUsersByEmail(string email);
        Task<User> GetUserById(int id);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<bool> AddOrEditUser(User user);
        Task<bool> DeleteUser(int id);
    }
    public class UserServices : IUserServices
    {
        private readonly EventManagementContext _context;

        public UserServices(EventManagementContext context)
        {
            _context = context;
        }

        public async Task<bool> AddOrEditUser(User user)
        {
            var entry = await _context.Users.FirstOrDefaultAsync(x => x.UserId == user.UserId);
            if (entry == null)
            {
                _context.Users.Add(user);
            }
            else
            {
                entry.Username = user.Username;
                entry.FullName = user.FullName;
                entry.Role = user.Role;
                entry.Email = user.Email;
                entry.Password = user.Password;
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
        }

        public async Task<bool> DeleteUser(int id)
        {
            var entry = await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
            if (entry != null)
            {
                _context.Users.Remove(entry);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.AsQueryable().ToListAsync();
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<List<User>> GetUsersByEmail(string email)
        {
            return await _context.Users.Where(x => x.Email.Contains(email)).ToListAsync();
        }
    }
}
