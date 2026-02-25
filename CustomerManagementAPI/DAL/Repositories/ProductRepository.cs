using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL.Repositories
{
    /// <summary>
    /// Repository implementation for Product entity
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(ApplicationDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all products with optional filtering by name
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllAsync(string? name = null)
        {
            return await GetFilteredAsync(name);
        }

        /// <summary>
        /// Get all products with optional filtering by name
        /// </summary>
        public async Task<IEnumerable<Product>> GetFilteredAsync(string? name = null)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                // Apply filter if provided
                if (!string.IsNullOrWhiteSpace(name))
                {
                    query = query.Where(p => p.Name.Contains(name));
                }

                return await query.OrderBy(p => p.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw;
            }
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Products.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving product with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        public async Task<Product> AddAsync(Product product)
        {
            try
            {
                product.CreatedAt = DateTime.UtcNow;
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new product");
                throw;
            }
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        public async Task<Product?> UpdateAsync(Product product)
        {
            try
            {
                var existingProduct = await _context.Products.FindAsync(product.Id);
                if (existingProduct == null)
                {
                    return null;
                }

                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.SKU = product.SKU;
                existingProduct.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return existingProduct;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating product with ID {product.Id}");
                throw;
            }
        }

        /// <summary>
        /// Delete a product by ID
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return false;
                }

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting product with ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Check if a product exists
        /// </summary>
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Products.AnyAsync(p => p.Id == id);
        }

        /// <summary>
        /// Check if a product has any orders (for referential integrity check)
        /// Products cannot be deleted if they are used in orders
        /// </summary>
        public async Task<bool> HasOrdersAsync(int productId)
        {
            try
            {
                return await _context.OrderItems.AnyAsync(oi => oi.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking orders for product ID {productId}");
                throw;
            }
        }
    }
}
