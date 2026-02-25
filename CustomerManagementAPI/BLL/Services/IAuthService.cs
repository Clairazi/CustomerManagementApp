using CustomerManagementAPI.BLL.DTOs;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Interface for authentication service operations
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Authenticate user and return JWT token
        /// </summary>
        Task<LoginResponseDto?> LoginAsync(LoginDto loginDto);

        /// <summary>
        /// Register a new user
        /// </summary>
        Task<UserDto?> RegisterAsync(RegisterDto registerDto);

        /// <summary>
        /// Validate user credentials
        /// </summary>
        Task<bool> ValidateUserAsync(string username, string password);

        /// <summary>
        /// Get user by ID
        /// </summary>
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}
