using CustomerManagementAPI.BLL.DTOs;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Interface for Customer service operations
    /// </summary>
    public interface ICustomerService
    {
        /// <summary>
        /// Get all customers with optional filtering
        /// </summary>
        Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(string? firstName = null, 
            string? lastName = null, string? email = null, string? phoneNumber = null);

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        Task<CustomerDto?> GetCustomerByIdAsync(int id);

        /// <summary>
        /// Create a new customer
        /// </summary>
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);

        /// <summary>
        /// Update an existing customer
        /// </summary>
        Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto);

        /// <summary>
        /// Delete a customer
        /// </summary>
        Task<bool> DeleteCustomerAsync(int id);

        /// <summary>
        /// Export customers to Excel
        /// </summary>
        Task<byte[]> ExportCustomersToExcelAsync(string? firstName = null, 
            string? lastName = null, string? email = null, string? phoneNumber = null);
    }
}