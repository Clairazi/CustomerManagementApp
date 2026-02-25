using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.DAL.Entities;
using CustomerManagementAPI.DAL.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Authentication service for login, registration, and JWT token management
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IUserRepository userRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Authenticate user and return JWT token
        /// </summary>
        public async Task<LoginResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
                
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", loginDto.Username);
                    return null;
                }

                var token = GenerateJwtToken(user);

                _logger.LogInformation("User logged in successfully: {Username}", loginDto.Username);

                return new LoginResponseDto
                {
                    Token = token,
                    Username = user.Username,
                    FullName = user.FullName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", loginDto.Username);
                throw;
            }
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        public async Task<UserDto?> RegisterAsync(RegisterDto registerDto)
        {
            try
            {
                // Check if username already exists
                if (await _userRepository.ExistsAsync(registerDto.Username))
                {
                    _logger.LogWarning("Registration failed - username already exists: {Username}", registerDto.Username);
                    return null;
                }

                // Hash the password with BCrypt (10 salt rounds)
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password, 10);

                var user = new User
                {
                    Username = registerDto.Username,
                    PasswordHash = passwordHash,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName
                };

                var createdUser = await _userRepository.AddAsync(user);

                _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);

                return new UserDto
                {
                    Id = createdUser.Id,
                    Username = createdUser.Username,
                    Email = createdUser.Email,
                    FullName = createdUser.FullName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for user: {Username}", registerDto.Username);
                throw;
            }
        }

        /// <summary>
        /// Validate user credentials
        /// </summary>
        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            try
            {
                var user = await _userRepository.GetByUsernameAsync(username);
                
                if (user == null)
                    return false;

                return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user: {Username}", username);
                throw;
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                
                if (user == null)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID: {Id}", id);
                throw;
            }
        }

        /// <summary>
        /// Generate JWT token for authenticated user
        /// </summary>
        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
            var issuer = jwtSettings["Issuer"] ?? "CustomerManagementAPI";
            var audience = jwtSettings["Audience"] ?? "CustomerManagementApp";
            var expirationHours = int.Parse(jwtSettings["ExpirationHours"] ?? "24");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
