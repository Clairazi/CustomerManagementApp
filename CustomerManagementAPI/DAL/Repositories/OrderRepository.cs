using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Order entity.
    /// Handles data access for orders with master-detail relationship (Order -> OrderItems).
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders with Customer and OrderItems (including Product details)
        /// </summary>
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .OrderByDescending(o => o.OrderDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all orders");
                throw;
            }
        }

        /// <summary>
        /// Get a single order by ID with full details
        /// </summary>
        public async Task<Order?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .FirstOrDefaultAsync(o => o.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Get orders with optional filtering
        /// </summary>
        public async Task<IEnumerable<Order>> GetFilteredAsync(int? orderId = null, 
            DateTime? dateFrom = null, DateTime? dateTo = null, int? customerId = null)
        {
            try
            {
                var query = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                    .AsQueryable();

                // Filter by order ID
                if (orderId.HasValue)
                {
                    query = query.Where(o => o.Id == orderId.Value);
                }

                // Filter by date range
                if (dateFrom.HasValue)
                {
                    query = query.Where(o => o.OrderDate >= dateFrom.Value);
                }

                if (dateTo.HasValue)
                {
                    // Include the entire end date by setting time to end of day
                    var endDate = dateTo.Value.Date.AddDays(1);
                    query = query.Where(o => o.OrderDate < endDate);
                }

                // Filter by customer ID
                if (customerId.HasValue)
                {
                    query = query.Where(o => o.CustomerId == customerId.Value);
                }

                return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving filtered orders");
                throw;
            }
        }

        /// <summary>
        /// Add a new order with its order items
        /// </summary>
        public async Task<Order> AddAsync(Order order)
        {
            try
            {
                order.CreatedAt = DateTime.UtcNow;
                
                // Calculate subtotals and total amount
                foreach (var item in order.OrderItems)
                {
                    item.Subtotal = item.Quantity * item.UnitPrice;
                }
                order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Reload with navigation properties
                return await GetByIdAsync(order.Id) ?? order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new order");
                throw;
            }
        }

        /// <summary>
        /// Update an existing order and its order items
        /// </summary>
        public async Task<Order?> UpdateAsync(Order order)
        {
            try
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (existingOrder == null)
                {
                    return null;
                }

                // Update order header
                existingOrder.CustomerId = order.CustomerId;
                existingOrder.OrderDate = order.OrderDate;
                existingOrder.Status = order.Status;
                existingOrder.UpdatedAt = DateTime.UtcNow;

                // Remove existing order items
                _context.OrderItems.RemoveRange(existingOrder.OrderItems);

                // Add new order items
                foreach (var item in order.OrderItems)
                {
                    item.OrderId = order.Id;
                    item.Subtotal = item.Quantity * item.UnitPrice;
                    existingOrder.OrderItems.Add(item);
                }

                // Recalculate total amount
                existingOrder.TotalAmount = existingOrder.OrderItems.Sum(oi => oi.Subtotal);

                await _context.SaveChangesAsync();

                // Reload with navigation properties
                return await GetByIdAsync(existingOrder.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with ID {order.Id}");
                throw;
            }
        }

        /// <summary>
        /// Delete an order by ID (cascade deletes OrderItems)
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return false;
                }

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Check if an order exists
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        /// <summary>
        /// Get all orders for a specific customer (for referential integrity checks)
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.CustomerId == customerId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for customer ID {customerId}");
                throw;
            }
        }

        /// <summary>
        /// Get all orders containing a specific product (for referential integrity checks)
        /// </summary>
        public async Task<IEnumerable<Order>> GetOrdersByProductIdAsync(int productId)
        {
            try
            {
                return await _context.Orders
                    .Where(o => o.OrderItems.Any(oi => oi.ProductId == productId))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting orders for product ID {productId}");
                throw;
            }
        }
    }
}
