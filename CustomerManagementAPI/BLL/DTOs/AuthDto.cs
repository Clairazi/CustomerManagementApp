using System.ComponentModel.DataAnnotations;

namespace CustomerManagementAPI.BLL.DTOs
{
    /// <summary>
    /// DTO for user login request
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Username for authentication
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for authentication
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for user registration request
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Username for the new account
        /// </summary>
        [Required(ErrorMessage = "Username is required")]
        [MaxLength(50)]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password for the new account
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Email address (optional)
        /// </summary>
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(200)]
        public string? Email { get; set; }

        /// <summary>
        /// Full name for display (optional)
        /// </summary>
        [MaxLength(100)]
        public string? FullName { get; set; }
    }

    /// <summary>
    /// DTO for login response with JWT token
    /// </summary>
    public class LoginResponseDto
    {
        /// <summary>
        /// JWT authentication token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// Username of the logged-in user
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the logged-in user
        /// </summary>
        public string? FullName { get; set; }
    }

    /// <summary>
    /// DTO for user information
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string? FullName { get; set; }
    }
}
