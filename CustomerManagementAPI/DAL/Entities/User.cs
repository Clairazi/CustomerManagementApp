using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementAPI.DAL.Entities
{
    /// <summary>
    /// User entity representing an application user for authentication
    /// </summary>
    [Table("Users")]
    public class User
    {
        /// <summary>
        /// Unique identifier for the user
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Unique username for login (Required)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// BCrypt hashed password (Required)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// User's email address
        /// </summary>
        [MaxLength(200)]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// User's full name for display purposes
        /// </summary>
        [MaxLength(100)]
        public string? FullName { get; set; }

        /// <summary>
        /// Date when the user record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the user record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
