using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.DAL.Entities;
using CustomerManagementAPI.DAL.Repositories;
using OfficeOpenXml;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Service implementation for Customer business logic
    /// </summary>
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(ICustomerRepository customerRepository, ILogger<CustomerService> logger)
        {
            _customerRepository = customerRepository;
            _logger = logger;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Get all customers with optional filtering
        /// </summary>
        public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(string? firstName = null, 
            string? lastName = null, string? email = null, string? phoneNumber = null)
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync(firstName, lastName, email, phoneNumber);
                return customers.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCustomersAsync");
                throw;
            }
        }

        /// <summary>
        /// Get a customer by ID
        /// </summary>
        public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id);
                return customer != null ? MapToDto(customer) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetCustomerByIdAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Create a new customer with validation
        /// </summary>
        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            try
            {
                // Validate required fields
                ValidateCustomerData(createCustomerDto.FirstName, createCustomerDto.LastName);

                var customer = new Customer
                {
                    FirstName = createCustomerDto.FirstName.Trim(),
                    LastName = createCustomerDto.LastName.Trim(),
                    Email = createCustomerDto.Email?.Trim(),
                    PhoneNumber = createCustomerDto.PhoneNumber?.Trim()
                };

                var createdCustomer = await _customerRepository.AddCustomerAsync(customer);
                _logger.LogInformation($"Customer created successfully with ID {createdCustomer.Id}");
                return MapToDto(createdCustomer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCustomerAsync");
                throw;
            }
        }

        /// <summary>
        /// Update an existing customer with validation
        /// </summary>
        public async Task<CustomerDto?> UpdateCustomerAsync(int id, UpdateCustomerDto updateCustomerDto)
        {
            try
            {
                // Validate required fields
                ValidateCustomerData(updateCustomerDto.FirstName, updateCustomerDto.LastName);

                var customer = new Customer
                {
                    Id = id,
                    FirstName = updateCustomerDto.FirstName.Trim(),
                    LastName = updateCustomerDto.LastName.Trim(),
                    Email = updateCustomerDto.Email?.Trim(),
                    PhoneNumber = updateCustomerDto.PhoneNumber?.Trim()
                };

                var updatedCustomer = await _customerRepository.UpdateCustomerAsync(customer);
                
                if (updatedCustomer != null)
                {
                    _logger.LogInformation($"Customer updated successfully with ID {id}");
                }
                
                return updatedCustomer != null ? MapToDto(updatedCustomer) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateCustomerAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete a customer
        /// </summary>
        public async Task<bool> DeleteCustomerAsync(int id)
        {
            try
            {
                var result = await _customerRepository.DeleteCustomerAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Customer deleted successfully with ID {id}");
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteCustomerAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Export customers to Excel with filters
        /// </summary>
        public async Task<byte[]> ExportCustomersToExcelAsync(string? firstName = null, 
            string? lastName = null, string? email = null, string? phoneNumber = null)
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync(firstName, lastName, email, phoneNumber);
                
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Customers");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "First Name";
                worksheet.Cells[1, 3].Value = "Last Name";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Phone Number";
                worksheet.Cells[1, 6].Value = "Created At";

                // Style headers
                using (var range = worksheet.Cells[1, 1, 1, 6])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                int row = 2;
                foreach (var customer in customers)
                {
                    worksheet.Cells[row, 1].Value = customer.Id;
                    worksheet.Cells[row, 2].Value = customer.FirstName;
                    worksheet.Cells[row, 3].Value = customer.LastName;
                    worksheet.Cells[row, 4].Value = customer.Email;
                    worksheet.Cells[row, 5].Value = customer.PhoneNumber;
                    worksheet.Cells[row, 6].Value = customer.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _logger.LogInformation($"Exported {customers.Count()} customers to Excel");
                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportCustomersToExcelAsync");
                throw;
            }
        }

        /// <summary>
        /// Validate customer data - FirstName and LastName are mandatory
        /// </summary>
        private void ValidateCustomerData(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
            {
                throw new ArgumentException("First name is required");
            }

            if (string.IsNullOrWhiteSpace(lastName))
            {
                throw new ArgumentException("Last name is required");
            }
        }

        /// <summary>
        /// Map Customer entity to CustomerDto
        /// </summary>
        private CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };
        }
    }
}