using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementAPI.DAL.Entities
{
    /// <summary>
    /// Order entity representing the master record in the master-detail relationship.
    /// Contains order header information linked to a Customer and contains multiple OrderItems.
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        /// <summary>
        /// Unique identifier for the order
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Customer - Every order must belong to a customer
        /// </summary>
        [Required]
        public int CustomerId { get; set; }

        /// <summary>
        /// Date when the order was placed
        /// </summary>
        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Total amount of the order (sum of all OrderItems)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Order status (e.g., Pending, Processing, Completed, Cancelled)
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Date when the order record was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when the order record was last updated
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties

        /// <summary>
        /// Reference to the Customer who placed this order (required relationship)
        /// </summary>
        public virtual Customer Customer { get; set; } = null!;

        /// <summary>
        /// Collection of OrderItems - the detail records in the master-detail relationship
        /// </summary>
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
