using CustomerManagementAPI.BLL.DTOs;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Interface for Product service operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Get all products with optional filtering by name
        /// </summary>
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? name = null);

        /// <summary>
        /// Get a product by ID
        /// </summary>
        Task<ProductDto?> GetProductByIdAsync(int id);

        /// <summary>
        /// Create a new product
        /// </summary>
        Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto);

        /// <summary>
        /// Update an existing product
        /// </summary>
        Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto);

        /// <summary>
        /// Delete a product
        /// </summary>
        Task<bool> DeleteProductAsync(int id);

        /// <summary>
        /// Export products to Excel
        /// </summary>
        Task<byte[]> ExportProductsToExcelAsync(string? name = null);

        /// <summary>
        /// Check if a product has any orders (for referential integrity)
        /// </summary>
        Task<bool> HasOrdersAsync(int productId);
    }
}
