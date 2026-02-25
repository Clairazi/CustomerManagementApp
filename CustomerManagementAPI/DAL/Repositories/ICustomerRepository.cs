using CustomerManagementAPI.DAL.Entities;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Interface for Customer repository operations
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Get all customers with optional filtering
        /// </summary>
        Task<IEnumerable<Customer>> GetAllCustomersAsync(string? firstName = null, string? lastName = null, 
            string? email = null, string? phoneNumber = null);

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        Task<Customer?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Add a new customer
        /// </summary>
        Task<Customer> AddCustomerAsync(Customer customer);

        /// <summary>
        /// Update an existing customer
        /// </summary>
        Task<Customer?> UpdateCustomerAsync(Customer customer);

        /// <summary>
        /// Delete a customer by ID
        /// </summary>
        Task<bool> DeleteCustomerAsync(int id);

        /// <summary>
        /// Check if a customer exists
        /// </summary>
        Task<bool> CustomerExistsAsync(int id);

        /// <summary>
        /// Check if a customer has any orders (for referential integrity)
        /// </summary>
        Task<bool> HasOrdersAsync(int customerId);
    }
}