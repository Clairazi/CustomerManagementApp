using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementAPI.DAL.Entities
{
    /// <summary>
    /// Product entity representing a product in the database
    /// </summary>
    [Table("Products")]
    public class Product
    {
        /// <summary>
        /// Unique identifier for the product
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Product name (Required)
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Product description
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Product price
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        /// <summary>
        /// Stock Keeping Unit (SKU)
        /// </summary>
        [MaxLength(50)]
        public string? SKU { get; set; }

        /// <summary>
        /// Date when the product record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the product record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
