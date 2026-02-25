using System.ComponentModel.DataAnnotations;

namespace CustomerManagementAPI.BLL.DTOs
{
    /// <summary>
    /// Data Transfer Object for Order (Read operations).
    /// Used to transfer order data including customer info and order items to clients.
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// Collection of order items (detail records)
        /// </summary>
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    /// <summary>
    /// Data Transfer Object for OrderItem (Read operations).
    /// Contains product information and calculated subtotal.
    /// </summary>
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for creating a new Order.
    /// Master-Detail structure: order header with a list of order items.
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// Customer ID - Required. The customer placing the order.
        /// </summary>
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Order Date - Defaults to current date if not provided
        /// </summary>
        public DateTime? OrderDate { get; set; }

        /// <summary>
        /// Order status - Defaults to "Pending" if not provided
        /// </summary>
        [MaxLength(50)]
        public string? Status { get; set; }

        /// <summary>
        /// List of order items - At least one item is required
        /// </summary>
        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }

    /// <summary>
    /// Data Transfer Object for creating a new OrderItem.
    /// Each item represents one product line in the order.
    /// </summary>
    public class CreateOrderItemDto
    {
        /// <summary>
        /// Product ID - Required. The product being ordered.
        /// </summary>
        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity ordered - Must be at least 1
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price at time of order
        /// </summary>
        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for updating an existing Order.
    /// Similar structure to CreateOrderDto.
    /// </summary>
    public class UpdateOrderDto
    {
        /// <summary>
        /// Customer ID - Required. The customer placing the order.
        /// </summary>
        [Required(ErrorMessage = "Customer is required")]
        public int CustomerId { get; set; }

        /// <summary>
        /// Order Date
        /// </summary>
        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Order status
        /// </summary>
        [Required(ErrorMessage = "Status is required")]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// List of order items - At least one item is required
        /// </summary>
        [Required(ErrorMessage = "At least one order item is required")]
        [MinLength(1, ErrorMessage = "At least one order item is required")]
        public List<UpdateOrderItemDto> OrderItems { get; set; } = new List<UpdateOrderItemDto>();
    }

    /// <summary>
    /// Data Transfer Object for updating an OrderItem.
    /// </summary>
    public class UpdateOrderItemDto
    {
        /// <summary>
        /// Product ID - Required
        /// </summary>
        [Required(ErrorMessage = "Product is required")]
        public int ProductId { get; set; }

        /// <summary>
        /// Quantity ordered
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        /// <summary>
        /// Unit price
        /// </summary>
        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        public decimal UnitPrice { get; set; }
    }
}
