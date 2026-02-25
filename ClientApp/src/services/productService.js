import axios from 'axios';

// Base API URL
const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Product API Service
 * Handles all API calls related to product operations
 */
class ProductService {
  /**
   * Get all products with optional name filter
   * @param {Object} filters - Filter parameters (name)
   * @returns {Promise} API response with product list
   */
  async getAllProducts(filters = {}) {
    try {
      const params = new URLSearchParams();
      
      if (filters.name) params.append('name', filters.name);

      const response = await axios.get(`${API_BASE_URL}/products?${params.toString()}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching products:', error);
      throw error;
    }
  }

  /**
   * Get a product by ID
   * @param {number} id - Product ID
   * @returns {Promise} API response with product details
   */
  async getProductById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/products/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching product ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new product
   * @param {Object} productData - Product data
   * @returns {Promise} API response with created product
   */
  async createProduct(productData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/products`, productData);
      return response.data;
    } catch (error) {
      console.error('Error creating product:', error);
      throw error;
    }
  }

  /**
   * Update an existing product
   * @param {number} id - Product ID
   * @param {Object} productData - Updated product data
   * @returns {Promise} API response with updated product
   */
  async updateProduct(id, productData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/products/${id}`, productData);
      return response.data;
    } catch (error) {
      console.error(`Error updating product ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete a product
   * @param {number} id - Product ID
   * @returns {Promise} API response
   */
  async deleteProduct(id) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/products/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error deleting product ${id}:`, error);
      throw error;
    }
  }

  /**
   * Export products to Excel
   * @param {Object} filters - Filter parameters
   * @returns {Promise} Blob for Excel file download
   */
  async exportProductsToExcel(filters = {}) {
    try {
      const params = new URLSearchParams();
      
      if (filters.name) params.append('name', filters.name);

      const response = await axios.get(`${API_BASE_URL}/products/export?${params.toString()}`, {
        responseType: 'blob'
      });
      
      // Create a download link
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Products_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      
      return response.data;
    } catch (error) {
      console.error('Error exporting products:', error);
      throw error;
    }
  }
}

export default new ProductService();
