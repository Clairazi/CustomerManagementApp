import React, { createContext, useContext, useState, useEffect } from 'react';
import authService from '../services/authService';

/**
 * Authentication Context
 * Manages authentication state throughout the application
 */
const AuthContext = createContext(null);

/**
 * Authentication Provider Component
 * Wraps the application and provides authentication state and methods
 */
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [token, setToken] = useState(authService.getToken());
  const [isAuthenticated, setIsAuthenticated] = useState(!!authService.getToken());
  const [loading, setLoading] = useState(true);

  // Check authentication status on mount
  useEffect(() => {
    checkAuth();
  }, []);

  /**
   * Check if user is authenticated and fetch user info
   */
  const checkAuth = async () => {
    const savedToken = authService.getToken();
    if (savedToken) {
      try {
        const userData = await authService.getCurrentUser();
        setUser(userData);
        setToken(savedToken);
        setIsAuthenticated(true);
      } catch (error) {
        // Token is invalid or expired
        console.error('Auth check failed:', error);
        authService.logout();
        setUser(null);
        setToken(null);
        setIsAuthenticated(false);
      }
    } else {
      setUser(null);
      setToken(null);
      setIsAuthenticated(false);
    }
    setLoading(false);
  };

  /**
   * Login user with credentials
   * @param {string} username 
   * @param {string} password 
   * @returns {Promise} login result
   */
  const login = async (username, password) => {
    try {
      const response = await authService.login(username, password);
      setUser({ username: response.username, fullName: response.fullName });
      setToken(response.token);
      setIsAuthenticated(true);
      return { success: true };
    } catch (error) {
      console.error('Login failed:', error);
      return { 
        success: false, 
        message: error.response?.data?.message || 'Login failed. Please try again.' 
      };
    }
  };

  /**
   * Register a new user
   * @param {Object} userData - User registration data
   * @returns {Promise} registration result
   */
  const register = async (userData) => {
    try {
      await authService.register(userData);
      return { success: true };
    } catch (error) {
      console.error('Registration failed:', error);
      return { 
        success: false, 
        message: error.response?.data?.message || 'Registration failed. Please try again.' 
      };
    }
  };

  /**
   * Logout the current user
   */
  const logout = () => {
    authService.logout();
    setUser(null);
    setToken(null);
    setIsAuthenticated(false);
  };

  const value = {
    user,
    token,
    isAuthenticated,
    loading,
    login,
    register,
    logout,
    checkAuth
  };

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

/**
 * Custom hook to use authentication context
 * @returns {Object} auth context value
 */
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}

export default AuthContext;
