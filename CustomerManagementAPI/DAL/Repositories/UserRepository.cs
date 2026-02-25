using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Repository for User data access operations
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get a user by username
        /// </summary>
        public async Task<User?> GetByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                throw;
            }
        }

        /// <summary>
        /// Get a user by ID
        /// </summary>
        public async Task<User?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Users.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Add a new user
        /// </summary>
        public async Task<User> AddAsync(User user)
        {
            try
            {
                user.CreatedAt = DateTime.UtcNow;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Username}", user.Username);
                throw;
            }
        }

        /// <summary>
        /// Check if a username already exists
        /// </summary>
        public async Task<bool> ExistsAsync(string username)
        {
            try
            {
                return await _context.Users
                    .AnyAsync(u => u.Username.ToLower() == username.ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists: {Username}", username);
                throw;
            }
        }
    }
}
