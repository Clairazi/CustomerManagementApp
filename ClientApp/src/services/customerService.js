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
 * Customer API Service
 * Handles all API calls related to customer operations
 * All requests include JWT authentication token
 */
class CustomerService {
  /**
   * Get all customers with optional filters
   * @param {Object} filters - Filter parameters (firstName, lastName, email, phoneNumber)
   * @returns {Promise} API response with customer list
   */
  async getAllCustomers(filters = {}) {
    try {
      const params = new URLSearchParams();
      
      if (filters.firstName) params.append('firstName', filters.firstName);
      if (filters.lastName) params.append('lastName', filters.lastName);
      if (filters.email) params.append('email', filters.email);
      if (filters.phoneNumber) params.append('phoneNumber', filters.phoneNumber);

      const response = await axios.get(`${API_BASE_URL}/customers?${params.toString()}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching customers:', error);
      handleApiError(error);
    }
  }

  /**
   * Get a customer by ID
   * @param {number} id - Customer ID
   * @returns {Promise} API response with customer details
   */
  async getCustomerById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/customers/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error fetching customer ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Create a new customer
   * @param {Object} customerData - Customer data
   * @returns {Promise} API response with created customer
   */
  async createCustomer(customerData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/customers`, customerData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error creating customer:', error);
      handleApiError(error);
    }
  }

  /**
   * Update an existing customer
   * @param {number} id - Customer ID
   * @param {Object} customerData - Updated customer data
   * @returns {Promise} API response with updated customer
   */
  async updateCustomer(id, customerData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/customers/${id}`, customerData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error updating customer ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Delete a customer
   * @param {number} id - Customer ID
   * @returns {Promise} API response
   */
  async deleteCustomer(id) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/customers/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error deleting customer ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Export customers to Excel
   * @param {Object} filters - Filter parameters
   * @returns {Promise} Blob for Excel file download
   */
  async exportCustomersToExcel(filters = {}) {
    try {
      const params = new URLSearchParams();
      
      if (filters.firstName) params.append('firstName', filters.firstName);
      if (filters.lastName) params.append('lastName', filters.lastName);
      if (filters.email) params.append('email', filters.email);
      if (filters.phoneNumber) params.append('phoneNumber', filters.phoneNumber);

      const response = await axios.get(`${API_BASE_URL}/customers/export?${params.toString()}`, {
        headers: getAuthHeaders(),
        responseType: 'blob'
      });
      
      // Create a download link
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `Customers_${new Date().toISOString().split('T')[0]}.xlsx`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      
      return response.data;
    } catch (error) {
      console.error('Error exporting customers:', error);
      handleApiError(error);
    }
  }
}

export default new CustomerService();
