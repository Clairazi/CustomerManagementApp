using CustomerManagementAPI.DAL.Entities;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Interface for Order repository operations.
    /// Provides data access methods for Order and OrderItem entities.
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Get all orders with Customer and OrderItems (including Product details)
        /// </summary>
        Task<IEnumerable<Order>> GetAllAsync();

        /// <summary>
        /// Get a single order by ID with full details (Customer, OrderItems, Products)
        /// </summary>
        Task<Order?> GetByIdAsync(int id);

        /// <summary>
        /// Get orders with optional filtering by orderId, date range, and customerId
        /// </summary>
        /// <param name="orderId">Filter by order ID (partial match not supported for integers)</param>
        /// <param name="dateFrom">Filter orders from this date</param>
        /// <param name="dateTo">Filter orders up to this date</param>
        /// <param name="customerId">Filter by customer ID</param>
        Task<IEnumerable<Order>> GetFilteredAsync(int? orderId = null, DateTime? dateFrom = null, 
            DateTime? dateTo = null, int? customerId = null);

        /// <summary>
        /// Add a new order with its order items
        /// </summary>
        Task<Order> AddAsync(Order order);

        /// <summary>
        /// Update an existing order and its order items
        /// </summary>
        Task<Order?> UpdateAsync(Order order);

        /// <summary>
        /// Delete an order by ID (cascade deletes OrderItems)
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if an order exists
        /// </summary>
        Task<bool> ExistsAsync(int id);

        /// <summary>
        /// Get all orders for a specific customer (for integrity checks)
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId);

        /// <summary>
        /// Get all orders containing a specific product (for integrity checks)
        /// </summary>
        Task<IEnumerable<Order>> GetOrdersByProductIdAsync(int productId);
    }
}
