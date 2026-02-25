using CustomerManagementAPI.DAL.Entities;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Interface for User repository operations
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Get a user by username
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Get a user by ID
        /// </summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Add a new user
        /// </summary>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Check if a username already exists
        /// </summary>
        Task<bool> ExistsAsync(string username);
    }
}
