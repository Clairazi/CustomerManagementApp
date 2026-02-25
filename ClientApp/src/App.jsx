import React, { useState } from 'react';
import { Container, Navbar, Spinner } from 'react-bootstrap';
import { AuthProvider, useAuth } from './context/AuthContext';
import CustomerList from './components/CustomerList';
import CustomerForm from './components/CustomerForm';
import ProductList from './components/ProductList';
import ProductForm from './components/ProductForm';
import OrderList from './components/OrderList';
import OrderForm from './components/OrderForm';
import Navigation from './components/Navigation';
import Login from './components/Login';
import Register from './components/Register';
import './App.css';

/**
 * Main Application Content
 * Handles the main application logic including navigation and CRUD operations
 * Protected by authentication - only visible when logged in
 */
function AppContent() {
  const { isAuthenticated, loading, user, logout } = useAuth();
  
  // Auth view state (login or register)
  const [authView, setAuthView] = useState('login');

  // Module navigation state (customers, products, or orders)
  const [currentModule, setCurrentModule] = useState('customers');
  
  // Customer state management
  const [customerView, setCustomerView] = useState('list');
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [customerRefreshTrigger, setCustomerRefreshTrigger] = useState(0);

  // Product state management
  const [productView, setProductView] = useState('list');
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [productRefreshTrigger, setProductRefreshTrigger] = useState(0);

  // Order state management (master-detail)
  const [orderView, setOrderView] = useState('list');
  const [selectedOrder, setSelectedOrder] = useState(null);
  const [orderRefreshTrigger, setOrderRefreshTrigger] = useState(0);

  // Show loading spinner while checking auth status
  if (loading) {
    return (
      <div className="d-flex align-items-center justify-content-center" style={{ minHeight: '100vh' }}>
        <Spinner animation="border" variant="primary" />
        <span className="ms-2">Loading...</span>
      </div>
    );
  }

  // Show login/register if not authenticated
  if (!isAuthenticated) {
    if (authView === 'register') {
      return (
        <Register 
          onLoginClick={() => setAuthView('login')}
          onSuccess={() => setAuthView('login')}
        />
      );
    }
    return <Login onRegisterClick={() => setAuthView('register')} />;
  }

  /**
   * Handle module change (customers/products/orders)
   */
  const handleModuleChange = (module) => {
    setCurrentModule(module);
    // Reset views when switching modules
    if (module === 'customers') {
      setCustomerView('list');
      setSelectedCustomer(null);
    } else if (module === 'products') {
      setProductView('list');
      setSelectedProduct(null);
    } else if (module === 'orders') {
      setOrderView('list');
      setSelectedOrder(null);
    }
  };

  // Customer handlers
  const handleAddNewCustomer = () => {
    setSelectedCustomer(null);
    setCustomerView('form');
  };

  const handleEditCustomer = (customer) => {
    setSelectedCustomer(customer);
    setCustomerView('form');
  };

  const handleCustomerSaveSuccess = () => {
    setCustomerView('list');
    setSelectedCustomer(null);
    setCustomerRefreshTrigger(prev => prev + 1);
  };

  const handleCustomerCancel = () => {
    setCustomerView('list');
    setSelectedCustomer(null);
  };

  const handleCustomerDeleteSuccess = () => {
    setCustomerRefreshTrigger(prev => prev + 1);
  };

  // Product handlers
  const handleAddNewProduct = () => {
    setSelectedProduct(null);
    setProductView('form');
  };

  const handleEditProduct = (product) => {
    setSelectedProduct(product);
    setProductView('form');
  };

  const handleProductSaveSuccess = () => {
    setProductView('list');
    setSelectedProduct(null);
    setProductRefreshTrigger(prev => prev + 1);
  };

  const handleProductCancel = () => {
    setProductView('list');
    setSelectedProduct(null);
  };

  const handleProductDeleteSuccess = () => {
    setProductRefreshTrigger(prev => prev + 1);
  };

  // Order handlers (master-detail)
  const handleAddNewOrder = () => {
    setSelectedOrder(null);
    setOrderView('form');
  };

  const handleEditOrder = (order) => {
    setSelectedOrder(order);
    setOrderView('form');
  };

  const handleOrderSaveSuccess = () => {
    setOrderView('list');
    setSelectedOrder(null);
    setOrderRefreshTrigger(prev => prev + 1);
  };

  const handleOrderCancel = () => {
    setOrderView('list');
    setSelectedOrder(null);
  };

  const handleOrderDeleteSuccess = () => {
    setOrderRefreshTrigger(prev => prev + 1);
  };

  /**
   * Render the appropriate content based on current module and view
   */
  const renderContent = () => {
    if (currentModule === 'customers') {
      if (customerView === 'list') {
        return (
          <CustomerList 
            onAddNew={handleAddNewCustomer}
            onEdit={handleEditCustomer}
            onDeleteSuccess={handleCustomerDeleteSuccess}
            refreshTrigger={customerRefreshTrigger}
          />
        );
      } else {
        return (
          <CustomerForm 
            customer={selectedCustomer}
            onSave={handleCustomerSaveSuccess}
            onCancel={handleCustomerCancel}
          />
        );
      }
    } else if (currentModule === 'products') {
      if (productView === 'list') {
        return (
          <ProductList 
            onAddNew={handleAddNewProduct}
            onEdit={handleEditProduct}
            onDeleteSuccess={handleProductDeleteSuccess}
            refreshTrigger={productRefreshTrigger}
          />
        );
      } else {
        return (
          <ProductForm 
            product={selectedProduct}
            onSave={handleProductSaveSuccess}
            onCancel={handleProductCancel}
          />
        );
      }
    } else if (currentModule === 'orders') {
      if (orderView === 'list') {
        return (
          <OrderList 
            onAddNew={handleAddNewOrder}
            onEdit={handleEditOrder}
            onDeleteSuccess={handleOrderDeleteSuccess}
            refreshTrigger={orderRefreshTrigger}
          />
        );
      } else {
        return (
          <OrderForm 
            order={selectedOrder}
            onSave={handleOrderSaveSuccess}
            onCancel={handleOrderCancel}
          />
        );
      }
    }
  };

  return (
    <div className="app-container">
      {/* Navigation Bar */}
      <Navbar bg="primary" variant="dark" expand="lg">
        <Container>
          <Navbar.Brand href="#home">
            <i className="bi bi-building me-2"></i>
            Customer, Product & Order Management System
          </Navbar.Brand>
          <Navbar.Toggle aria-controls="navbar-nav" />
          <Navbar.Collapse id="navbar-nav" className="justify-content-end">
            <Navbar.Text className="me-3">
              <i className="bi bi-person-circle me-1"></i>
              Welcome, <strong>{user?.fullName || user?.username || 'User'}</strong>
            </Navbar.Text>
            <button 
              className="btn btn-outline-light btn-sm" 
              onClick={logout}
            >
              <i className="bi bi-box-arrow-right me-1"></i>
              Logout
            </button>
          </Navbar.Collapse>
        </Container>
      </Navbar>

      {/* Module Navigation */}
      <Navigation 
        currentModule={currentModule}
        onModuleChange={handleModuleChange}
        user={user}
        onLogout={logout}
      />

      {/* Main Content */}
      <Container className="main-content">
        {renderContent()}
      </Container>

      {/* Footer */}
      <footer className="bg-light text-center py-3 mt-5">
        <Container>
          <p className="mb-0 text-muted">
            &copy; {new Date().getFullYear()} Customer, Product & Order Management System. All rights reserved.
          </p>
        </Container>
      </footer>
    </div>
  );
}

/**
 * Main App Component
 * Wraps the application with AuthProvider for authentication context
 */
function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;
