using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerManagementAPI.DAL.Entities
{
    [Table("OrderItems")]
    public class OrderItem
    {
        /// <summary>
        /// Unique identifier for the order item
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Order - Every order item must belong to an order
        /// </summary>
        [Required]
        public int OrderId { get; set; }

        /// <summary>
        /// Foreign key to Product - Every order item must reference a product
        /// </summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity of the product ordered
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price of the product at the time of order
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Subtotal for this line item (Quantity * UnitPrice)
        /// </summary>
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        // Navigation Properties

        /// <summary>
        /// Reference to the parent Order
        /// </summary>
        public virtual Order Order { get; set; } = null!;

        /// <summary>
        /// Reference to the Product being ordered
        /// </summary>
        public virtual Product Product { get; set; } = null!;
    }
}
