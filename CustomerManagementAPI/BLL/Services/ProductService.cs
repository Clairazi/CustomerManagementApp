using CustomerManagementAPI.BLL.DTOs;
using CustomerManagementAPI.DAL.Entities;
using CustomerManagementAPI.DAL.Repositories;
using OfficeOpenXml;

namespace CustomerManagementAPI.BLL.Services
{
    /// <summary>
    /// Service implementation for Product business logic
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
            
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        /// <summary>
        /// Get all products with optional filtering by name
        /// </summary>
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(string? name = null)
        {
            try
            {
                var products = await _productRepository.GetAllAsync(name);
                return products.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllProductsAsync");
                throw;
            }
        }

        /// <summary>
        /// Get a product by ID
        /// </summary>
        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(id);
                return product != null ? MapToDto(product) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in GetProductByIdAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Create a new product with validation
        /// </summary>
        public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                // Validate required fields
                ValidateProductData(createProductDto.Name);

                var product = new Product
                {
                    Name = createProductDto.Name.Trim(),
                    Description = createProductDto.Description?.Trim(),
                    Price = createProductDto.Price,
                    SKU = createProductDto.SKU?.Trim()
                };

                var createdProduct = await _productRepository.AddAsync(product);
                _logger.LogInformation($"Product created successfully with ID {createdProduct.Id}");
                return MapToDto(createdProduct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateProductAsync");
                throw;
            }
        }

        /// <summary>
        /// Update an existing product with validation
        /// </summary>
        public async Task<ProductDto?> UpdateProductAsync(int id, UpdateProductDto updateProductDto)
        {
            try
            {
                // Validate required fields
                ValidateProductData(updateProductDto.Name);

                var product = new Product
                {
                    Id = id,
                    Name = updateProductDto.Name.Trim(),
                    Description = updateProductDto.Description?.Trim(),
                    Price = updateProductDto.Price,
                    SKU = updateProductDto.SKU?.Trim()
                };

                var updatedProduct = await _productRepository.UpdateAsync(product);
                
                if (updatedProduct != null)
                {
                    _logger.LogInformation($"Product updated successfully with ID {id}");
                }
                
                return updatedProduct != null ? MapToDto(updatedProduct) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in UpdateProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Delete a product with referential integrity check.
        /// Products cannot be deleted if they are used in orders.
        /// </summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                // Check referential integrity - product cannot be deleted if it's used in orders
                var hasOrders = await _productRepository.HasOrdersAsync(id);
                if (hasOrders)
                {
                    throw new InvalidOperationException($"Cannot delete product with ID {id}. Product is used in existing orders. Please delete the orders first.");
                }

                var result = await _productRepository.DeleteAsync(id);
                if (result)
                {
                    _logger.LogInformation($"Product deleted successfully with ID {id}");
                }
                return result;
            }
            catch (InvalidOperationException)
            {
                throw; // Re-throw integrity violation
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in DeleteProductAsync for ID {id}");
                throw;
            }
        }

        /// <summary>
        /// Check if a product has any orders (for referential integrity)
        /// </summary>
        public async Task<bool> HasOrdersAsync(int productId)
        {
            try
            {
                return await _productRepository.HasOrdersAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking orders for product ID {productId}");
                throw;
            }
        }

        /// <summary>
        /// Export products to Excel with filters
        /// </summary>
        public async Task<byte[]> ExportProductsToExcelAsync(string? name = null)
        {
            try
            {
                var products = await _productRepository.GetAllAsync(name);
                
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Products");

                // Add headers
                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Name";
                worksheet.Cells[1, 3].Value = "Description";
                worksheet.Cells[1, 4].Value = "Price";
                worksheet.Cells[1, 5].Value = "SKU";
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
                foreach (var product in products)
                {
                    worksheet.Cells[row, 1].Value = product.Id;
                    worksheet.Cells[row, 2].Value = product.Name;
                    worksheet.Cells[row, 3].Value = product.Description;
                    worksheet.Cells[row, 4].Value = product.Price;
                    worksheet.Cells[row, 4].Style.Numberformat.Format = "$#,##0.00";
                    worksheet.Cells[row, 5].Value = product.SKU;
                    worksheet.Cells[row, 6].Value = product.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
                    row++;
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                _logger.LogInformation($"Exported {products.Count()} products to Excel");
                return package.GetAsByteArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ExportProductsToExcelAsync");
                throw;
            }
        }

        /// <summary>
        /// Validate product data - Name is mandatory
        /// </summary>
        private void ValidateProductData(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Product name is required");
            }
        }

        /// <summary>
        /// Map Product entity to ProductDto
        /// </summary>
        private ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                SKU = product.SKU,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }
    }
}
