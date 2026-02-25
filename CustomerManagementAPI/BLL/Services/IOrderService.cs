using CustomerManagementAPI.BLL.DTOs;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Interface for Order service operations.
    /// Defines business logic operations for the Order module with master-detail functionality.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Get all orders with optional filtering by orderId, date range, and customerId
        /// </summary>
        /// <param name="orderId">Filter by specific order ID</param>
        /// <param name="dateFrom">Filter orders from this date</param>
        /// <param name="dateTo">Filter orders up to this date</param>
        /// <param name="customerId">Filter by customer ID</param>
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int? orderId = null, DateTime? dateFrom = null, 
            DateTime? dateTo = null, int? customerId = null);

        /// <summary>
        /// Get a single order by ID with full details (customer info, order items with products)
        /// </summary>
        Task<OrderDto?> GetOrderByIdAsync(int id);

        /// <summary>
        /// Create a new order with order items.
        /// Validates that customer exists and all products exist.
        /// Automatically calculates subtotals and total amount.
        /// </summary>
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto);

        /// <summary>
        /// Update an existing order and its order items.
        /// Validates customer and products exist.
        /// Recalculates totals automatically.
        /// </summary>
        Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto);

        /// <summary>
        /// Delete an order by ID (cascade deletes order items)
        /// </summary>
        Task<bool> DeleteOrderAsync(int id);

        /// <summary>
        /// Check if a customer has any orders (for referential integrity)
        /// </summary>
        Task<bool> CustomerHasOrdersAsync(int customerId);

        /// <summary>
        /// Check if a product is used in any orders (for referential integrity)
        /// </summary>
        Task<bool> ProductHasOrdersAsync(int productId);
    }
}
