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
 * Order API Service
 * Handles all API calls related to order operations with master-detail functionality
 * All requests include JWT authentication token
 */
class OrderService {
  /**
   * Get all orders with optional filters
   * @param {Object} filters - Filter parameters (orderId, dateFrom, dateTo, customerId)
   * @returns {Promise} API response with order list
   */
  async getAllOrders(filters = {}) {
    try {
      const params = new URLSearchParams();
      
      if (filters.orderId) params.append('orderId', filters.orderId);
      if (filters.dateFrom) params.append('dateFrom', filters.dateFrom);
      if (filters.dateTo) params.append('dateTo', filters.dateTo);
      if (filters.customerId) params.append('customerId', filters.customerId);

      const response = await axios.get(`${API_BASE_URL}/orders?${params.toString()}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error fetching orders:', error);
      handleApiError(error);
    }
  }

  /**
   * Get an order by ID with full details (customer, order items, products)
   * @param {number} id - Order ID
   * @returns {Promise} API response with order details
   */
  async getOrderById(id) {
    try {
      const response = await axios.get(`${API_BASE_URL}/orders/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error fetching order ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Create a new order with order items (master-detail)
   * @param {Object} orderData - Order data including orderItems array
   * @returns {Promise} API response with created order
   */
  async createOrder(orderData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/orders`, orderData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error('Error creating order:', error);
      handleApiError(error);
    }
  }

  /**
   * Update an existing order with order items
   * @param {number} id - Order ID
   * @param {Object} orderData - Updated order data including orderItems array
   * @returns {Promise} API response with updated order
   */
  async updateOrder(id, orderData) {
    try {
      const response = await axios.put(`${API_BASE_URL}/orders/${id}`, orderData, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error updating order ${id}:`, error);
      handleApiError(error);
    }
  }

  /**
   * Delete an order (cascade deletes order items)
   * @param {number} id - Order ID
   * @returns {Promise} API response
   */
  async deleteOrder(id) {
    try {
      const response = await axios.delete(`${API_BASE_URL}/orders/${id}`, {
        headers: getAuthHeaders()
      });
      return response.data;
    } catch (error) {
      console.error(`Error deleting order ${id}:`, error);
      handleApiError(error);
    }
  }
}

export default new OrderService();
