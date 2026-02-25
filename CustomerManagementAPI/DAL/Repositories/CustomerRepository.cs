using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Customer entity
    /// </summary>
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerRepository> _logger;

        public CustomerRepository(ApplicationDbContext context, ILogger<CustomerRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all customers with optional filtering
        /// </summary>
        public async Task<IEnumerable<Customer>> GetAllCustomersAsync(string? firstName = null, 
            string? lastName = null, string? email = null, string? phoneNumber = null)
        {
            try
            {
                var query = _context.Customers.AsQueryable();

                // Apply filters if provided
                if (!string.IsNullOrWhiteSpace(firstName))
                {
                    query = query.Where(c => c.FirstName.Contains(firstName));
                }

                if (!string.IsNullOrWhiteSpace(lastName))
                {
                    query = query.Where(c => c.LastName.Contains(lastName));
                }

                if (!string.IsNullOrWhiteSpace(email))
                {
                    query = query.Where(c => c.Email != null && c.Email.Contains(email));
                }

                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    query = query.Where(c => c.PhoneNumber != null && c.PhoneNumber.Contains(phoneNumber));
                }

                return await query.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customers");
                throw;
            }
        }

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            try
            {
                return await _context.Customers.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving customer with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Add a new customer
        /// </summary>
        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            try
            {
                customer.CreatedAt = DateTime.UtcNow;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new customer");
                throw;
            }
        }

        /// <summary>
        /// Update an existing customer
        /// </summary>
        public async Task<Customer?> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                var existingCustomer = await _context.Customers.FindAsync(customer.Id);
                if (existingCustomer == null)
                {
                    return null;
                }

                existingCustomer.FirstName = customer.FirstName;
                existingCustomer.LastName = customer.LastName;
                existingCustomer.Email = customer.Email;
                existingCustomer.PhoneNumber = customer.PhoneNumber;
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingCustomer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating customer with ID {customer.Id}");
                throw;
            }
        }

        /// <summary>
        /// Delete a customer by ID
        /// </summary>
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var customer = await _context.Customers.FindAsync(id);
                if (customer == null)
                {
                    return false;
                }

                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting customer with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Check if a customer exists
        /// </summary>
        public async Task<bool> CustomerExistsAsync(int id)
        {
            return await _context.Customers.AnyAsync(c => c.Id == id);
        }

        /// <summary>
        /// Check if a customer has any orders (for referential integrity check)
        /// Customers cannot be deleted if they have orders
        /// </summary>
        public async Task<bool> HasOrdersAsync(int customerId)
        {
            try
            {
                return await _context.Orders.AnyAsync(o => o.CustomerId == customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking orders for customer ID {customerId}");
                throw;
            }
        }
    }
}