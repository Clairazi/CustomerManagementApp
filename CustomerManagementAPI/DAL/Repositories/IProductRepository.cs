using CustomerManagementAPI.DAL.Entities;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Interface for Product repository operations
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        /// Get all products with optional filtering by name
        /// </summary>
        Task<IEnumerable<Product>> GetAllAsync(string? name = null);

        /// <summary>
        /// Get all products with optional filtering (alias for GetAllAsync)
        /// </summary>
        Task<IEnumerable<Product>> GetFilteredAsync(string? name = null);

        /// <summary>
        /// Get a product by ID
        /// </summary>
        Task<Product?> GetByIdAsync(int id);

        /// <summary>
        /// Add a new product
        /// </summary>
        Task<Product> AddAsync(Product product);

        /// <summary>
        /// Update an existing product
        /// </summary>
        Task<Product?> UpdateAsync(Product product);

        /// <summary>
        /// Delete a product by ID
        /// </summary>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Check if a product exists
        /// </summary>
        Task<bool> ExistsAsync(int id);
    }
}
