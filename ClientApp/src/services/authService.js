import axios from 'axios';

// Base API URL
const API_BASE_URL = 'http://localhost:5000/api';

// Token storage key
const TOKEN_KEY = 'auth_token';
const USER_KEY = 'auth_user';

/**
 * Authentication API Service
 * Handles all authentication-related API calls and token management
 */
class AuthService {
  /**
   * Login with username and password
   * @param {string} username 
   * @param {string} password 
   * @returns {Promise} Login response with token
   */
  async login(username, password) {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/login`, {
        username,
        password
      });

      if (response.data.token) {
        this.setToken(response.data.token);
        this.setUser({
          username: response.data.username,
          fullName: response.data.fullName
        });
      }

      return response.data;
    } catch (error) {
      console.error('Login error:', error);
      throw error;
    }
  }

  /**
   * Register a new user
   * @param {Object} userData - Registration data (username, password, email, fullName)
   * @returns {Promise} Registration response
   */
  async register(userData) {
    try {
      const response = await axios.post(`${API_BASE_URL}/auth/register`, userData);
      return response.data;
    } catch (error) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  /**
   * Get current user information (requires valid token)
   * @returns {Promise} Current user data
   */
  async getCurrentUser() {
    try {
      const token = this.getToken();
      if (!token) {
        throw new Error('No token found');
      }

      const response = await axios.get(`${API_BASE_URL}/auth/me`, {
        headers: {
          Authorization: `Bearer ${token}`
        }
      });
      return response.data;
    } catch (error) {
      console.error('Get current user error:', error);
      throw error;
    }
  }

  /**
   * Logout - Clear stored credentials
   */
  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  /**
   * Get stored JWT token
   * @returns {string|null} JWT token
   */
  getToken() {
    return localStorage.getItem(TOKEN_KEY);
  }

  /**
   * Store JWT token
   * @param {string} token 
   */
  setToken(token) {
    localStorage.setItem(TOKEN_KEY, token);
  }

  /**
   * Remove JWT token
   */
  removeToken() {
    localStorage.removeItem(TOKEN_KEY);
  }

  /**
   * Get stored user info
   * @returns {Object|null} User info
   */
  getUser() {
    const user = localStorage.getItem(USER_KEY);
    return user ? JSON.parse(user) : null;
  }

  /**
   * Store user info
   * @param {Object} user 
   */
  setUser(user) {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  }
}

export default new AuthService();
