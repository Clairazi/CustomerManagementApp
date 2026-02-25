using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerManagementAPI.Controllers
{
    /// <summary>
    /// API Controller for Customer management operations
    /// Requires authentication to access all endpoints
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomersController> _logger;

        public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
        {
            _customerService = customerService;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/customers - Get all customers with optional filtering
        /// </summary>
        /// <param name="firstName">Filter by first name</param>
        /// <param name="lastName">Filter by last name</param>
        /// <param name="email">Filter by email</param>
        /// <param name="phoneNumber">Filter by phone number</param>
        /// <returns>List of customers</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phoneNumber = null)
        {
            try
            {
                var customers = await _customerService.GetAllCustomersAsync(firstName, lastName, email, phoneNumber);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                return StatusCode(500, new { message = "An error occurred while retrieving customers", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/customers/{id} - Get a customer by ID
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Customer details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerService.GetCustomerByIdAsync(id);
                
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID {id} not found" });
                }

                return Ok(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer with ID {id}");
                return StatusCode(500, new { message = "An error occurred while retrieving the customer", error = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/customers - Create a new customer
        /// </summary>
        /// <param name="createCustomerDto">Customer data</param>
        /// <returns>Created customer</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto createCustomerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = await _customerService.CreateCustomerAsync(createCustomerDto);
                return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating customer");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating customer");
                return StatusCode(500, new { message = "An error occurred while creating the customer", error = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/customers/{id} - Update an existing customer
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <param name="updateCustomerDto">Updated customer data</param>
        /// <returns>Updated customer</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var customer = await _customerService.UpdateCustomerAsync(id, updateCustomerDto);
                
                if (customer == null)
                {
                    return NotFound(new { message = $"Customer with ID {id} not found" });
                }

                return Ok(customer);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while updating customer");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer with ID {id}");
                return StatusCode(500, new { message = "An error occurred while updating the customer", error = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/customers/{id} - Delete a customer
        /// Referential integrity: Returns 400 Bad Request if customer has orders
        /// </summary>
        /// <param name="id">Customer ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                var result = await _customerService.DeleteCustomerAsync(id);
                
                if (!result)
                {
                    return NotFound(new { message = $"Customer with ID {id} not found" });
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                // Referential integrity violation - customer has orders
                _logger.LogWarning(ex, $"Cannot delete customer with ID {id} - integrity violation");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with ID {id}");
                return StatusCode(500, new { message = "An error occurred while deleting the customer", error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/customers/export - Export customers to Excel
        /// </summary>
        /// <param name="firstName">Filter by first name</param>
        /// <param name="lastName">Filter by last name</param>
        /// <param name="email">Filter by email</param>
        /// <param name="phoneNumber">Filter by phone number</param>
        /// <returns>Excel file</returns>
        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExportCustomers(
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? email = null,
            [FromQuery] string? phoneNumber = null)
        {
            try
            {
                var excelData = await _customerService.ExportCustomersToExcelAsync(firstName, lastName, email, phoneNumber);
                var fileName = $"Customers_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customers to Excel");
                return StatusCode(500, new { message = "An error occurred while exporting customers", error = ex.Message });
            }
        }
    }
}
