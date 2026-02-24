import axios from 'axios';

// Base API URL
const API_BASE_URL = 'http://localhost:5000/api';

/**
 * Customer API Service
 * Handles all API calls related to customer operations
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

      const response = await axios.get(`${API_BASE_URL}/customers?${params.toString()}`);
      return response.data;
    } catch (error) {
      console.error('Error fetching customers:', error);
      throw error;
    }
  }

  /**
   * Get a customer by ID
   * @param {number} id - Customer ID
   * @returns {Promise} API response with customer details
   */
  async getCustomerById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/customers/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error fetching customer ${id}:`, error);
      throw error;
    }
  }

  /**
   * Create a new customer
   * @param {Object} customerData - Customer data
   * @returns {Promise} API response with created customer
   */
  async createCustomer(customerData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/customers`, customerData);
      return response.data;
    } catch (error) {
      console.error('Error creating customer:', error);
      throw error;
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
      const response = await axios.put(`${API_BASE_URL}/customers/${id}`, customerData);
      return response.data;
    } catch (error) {
      console.error(`Error updating customer ${id}:`, error);
      throw error;
    }
  }

  /**
   * Delete a customer
   * @param {number} id - Customer ID
   * @returns {Promise} API response
   */
  async deleteCustomer(id) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/customers/${id}`);
      return response.data;
    } catch (error) {
      console.error(`Error deleting customer ${id}:`, error);
      throw error;
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
      throw error;
    }
  }
}

export default new CustomerService();
