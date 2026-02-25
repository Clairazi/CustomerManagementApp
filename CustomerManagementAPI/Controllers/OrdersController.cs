using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementAPI.Controllers
{
    /// <summary>
    /// API Controller for Order management operations.
    /// Provides endpoints for master-detail order operations.
    /// Requires authentication to access all endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/orders - Get all orders with optional filtering
        /// </summary>
        /// <param name="orderId">Filter by order ID</param>
        /// <param name="dateFrom">Filter orders from this date</param>
        /// <param name="dateTo">Filter orders up to this date</param>
        /// <param name="customerId">Filter by customer ID</param>
        /// <returns>List of orders with customer and order items</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders(
            [FromQuery] int? orderId = null,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int? customerId = null)
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync(orderId, dateFrom, dateTo, customerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders");
                return StatusCode(500, new { message = "An error occurred while retrieving orders", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/orders/{id} - Get a single order by ID with full details
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Order details including customer info and order items</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                
                if (order == null)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving order with ID {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the order", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/orders - Create a new order with order items (master-detail)
        /// </summary>
        /// <param name="createOrderDto">Order data with order items</param>
        /// <returns>Created order</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto);
                return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating order");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/orders/{id} - Update an existing order
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <param name="updateOrderDto">Updated order data with order items</param>
        /// <returns>Updated order</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OrderDto>> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var order = await _orderService.UpdateOrderAsync(id, updateOrderDto);
                
                if (order == null)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating order");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order with ID {id}");
                return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/orders/{id} - Delete an order (cascade deletes order items)
        /// </summary>
        /// <param name="id">Order ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                var result = await _orderService.DeleteOrderAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = $"Order with ID {id} not found" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting order with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the order", error = ex.Message });
            }
        }
    }
}
