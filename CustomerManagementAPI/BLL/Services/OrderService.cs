using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.DAL.Entities;
using CustomerManagementAPI.DAL.Repositories;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Service implementation for Order business logic.
    /// Handles master-detail order operations with validation and integrity checks.
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            ICustomerRepository customerRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all orders with optional filtering
        /// </summary>
        public async Task<IEnumerable<OrderDto>> GetAllOrdersAsync(int? orderId = null, 
            DateTime? dateFrom = null, DateTime? dateTo = null, int? customerId = null)
        {
            try
            {
                var orders = await _orderRepository.GetFilteredAsync(orderId, dateFrom, dateTo, customerId);
                return orders.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllOrdersAsync");
                throw;
            }
        }

        /// <summary>
        /// Get a single order by ID with full details
        /// </summary>
        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderRepository.GetByIdAsync(id);
                return order != null ? MapToDto(order) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetOrderByIdAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Create a new order with validation.
        /// Validates that customer exists and all products exist.
        /// </summary>
        public async Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto)
        {
            try
            {
                // Validate customer exists
                var customerExists = await _customerRepository.CustomerExistsAsync(createOrderDto.CustomerId);
                if (!customerExists)
                {
                    throw new ArgumentException($"Customer with ID {createOrderDto.CustomerId} does not exist");
                }

                // Validate all products exist
                foreach (var item in createOrderDto.OrderItems)
                {
                    var productExists = await _productRepository.ExistsAsync(item.ProductId);
                    if (!productExists)
                    {
                        throw new ArgumentException($"Product with ID {item.ProductId} does not exist");
                    }
                }

                // Validate at least one order item
                if (createOrderDto.OrderItems == null || !createOrderDto.OrderItems.Any())
                {
                    throw new ArgumentException("At least one order item is required");
                }

                // Create order entity
                var order = new Order
                {
                    CustomerId = createOrderDto.CustomerId,
                    OrderDate = createOrderDto.OrderDate ?? DateTime.UtcNow,
                    Status = createOrderDto.Status ?? "Pending",
                    OrderItems = createOrderDto.OrderItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Quantity * item.UnitPrice
                    }).ToList()
                };

                // Calculate total amount
                order.TotalAmount = order.OrderItems.Sum(oi => oi.Subtotal);

                var createdOrder = await _orderRepository.AddAsync(order);
                _logger.LogInformation($"Order created successfully with ID {createdOrder.Id}");
                
                return MapToDto(createdOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateOrderAsync");
                throw;
            }
        }

        /// <summary>
        /// Update an existing order with validation
        /// </summary>
        public async Task<OrderDto?> UpdateOrderAsync(int id, UpdateOrderDto updateOrderDto)
        {
            try
            {
                // Validate customer exists
                var customerExists = await _customerRepository.CustomerExistsAsync(updateOrderDto.CustomerId);
                if (!customerExists)
                {
                    throw new ArgumentException($"Customer with ID {updateOrderDto.CustomerId} does not exist");
                }

                // Validate all products exist
                foreach (var item in updateOrderDto.OrderItems)
                {
                    var productExists = await _productRepository.ExistsAsync(item.ProductId);
                    if (!productExists)
                    {
                        throw new ArgumentException($"Product with ID {item.ProductId} does not exist");
                    }
                }

                // Validate at least one order item
                if (updateOrderDto.OrderItems == null || !updateOrderDto.OrderItems.Any())
                {
                    throw new ArgumentException("At least one order item is required");
                }

                // Create order entity for update
                var order = new Order
                {
                    Id = id,
                    CustomerId = updateOrderDto.CustomerId,
                    OrderDate = updateOrderDto.OrderDate,
                    Status = updateOrderDto.Status,
                    OrderItems = updateOrderDto.OrderItems.Select(item => new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Subtotal = item.Quantity * item.UnitPrice
                    }).ToList()
                };

                var updatedOrder = await _orderRepository.UpdateAsync(order);
                
                if (updatedOrder != null)
                {
                    _logger.LogInformation($"Order updated successfully with ID {id}");
                    return MapToDto(updatedOrder);
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateOrderAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete an order by ID
        /// </summary>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var result = await _orderRepository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Order deleted successfully with ID {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteOrderAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Check if a customer has any orders (for referential integrity)
        /// </summary>
        public async Task<bool> CustomerHasOrdersAsync(int customerId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByCustomerIdAsync(customerId);
                return orders.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking orders for customer ID {customerId}");
                throw;
            }
        }

        /// <summary>
        /// Check if a product is used in any orders (for referential integrity)
        /// </summary>
        public async Task<bool> ProductHasOrdersAsync(int productId)
        {
            try
            {
                var orders = await _orderRepository.GetOrdersByProductIdAsync(productId);
                return orders.Any();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking orders for product ID {productId}");
                throw;
            }
        }

        /// <summary>
        /// Map Order entity to OrderDto with all nested data
        /// </summary>
        private OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null 
                    ? $"{order.Customer.FirstName} {order.Customer.LastName}" 
                    : string.Empty,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? string.Empty,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    Subtotal = oi.Subtotal
                }).ToList()
            };
        }
    }
}
