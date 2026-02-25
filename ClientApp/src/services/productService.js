import axios from 'axios';
import authService from './authService';

// Base API URL
const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Get authorization headers with JWT token
 * @returns {Object} headers object
 */
const getAuthHeaders = () => {
  const token = authService.getToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};

/**
 * Handle API errors including 401 Unauthorized
 * @param {Error} error 
 */
const handleApiError = (error) => {
  if (error.response?.status === 401) {
    // Token expired or invalid - logout and redirect
    authService.logout();
    window.location.href = '/';
  }
  throw error;
};

/**
 * Product API Service
 * Handles all API calls related to product operations
 * All requests include JWT authentication token
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

      const response = await axios.get(`${API_BASE_URL}/products?${params.toString()}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching products:', error);
      handleApiError(error);
    }
  }

  /**
   * Get a product by ID
   * @param {number} id - Product ID
   * @returns {Promise} API response with product details
   */
  async getProductById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/products/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error fetching product ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Create a new product
   * @param {Object} productData - Product data
   * @returns {Promise} API response with created product
   */
  async createProduct(productData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/products`, productData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error creating product:', error);
      handleApiError(error);
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
      const response = await axios.put(`${API_BASE_URL}/products/${id}`, productData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error updating product ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Delete a product
   * @param {number} id - Product ID
   * @returns {Promise} API response
   */
  async deleteProduct(id) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/products/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error deleting product ${id}:`, error);
      handleApiError(error);
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
        headers: getAuthHeaders(),
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
      handleApiError(error);
    }
  }
}

export default new ProductService();
