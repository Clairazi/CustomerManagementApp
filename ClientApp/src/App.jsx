import React, { useState } from 'react';
import { Container, Navbar, Nav } from 'react-bootstrap';
import CustomerList from './components/CustomerList';
import CustomerForm from './components/CustomerForm';
import './App.css';

/**
 * Main App Component
 * Manages the overall application state and navigation
 */
function App() {
  const [currentView, setCurrentView] = useState('list');
  const [selectedCustomer, setSelectedCustomer] = useState(null);
  const [refreshTrigger, setRefreshTrigger] = useState(0);

  /**
   * Handle switching to add new customer view
   */
  const handleAddNew = () => {
    setSelectedCustomer(null);
    setCurrentView('form');
  };

  /**
   * Handle switching to edit customer view
   * @param {Object} customer - Customer to edit
   */
  const handleEdit = (customer) => {
    setSelectedCustomer(customer);
    setCurrentView('form');
  };

  /**
   * Handle successful save operation
   * Refresh the list and return to list view
   */
  const handleSaveSuccess = () => {
    setCurrentView('list');
    setSelectedCustomer(null);
    setRefreshTrigger(prev => prev + 1);
  };

  /**
   * Handle cancel operation
   * Return to list view without saving
   */
  const handleCancel = () => {
    setCurrentView('list');
    setSelectedCustomer(null);
  };

  /**
   * Handle successful delete operation
   * Refresh the list
   */
  const handleDeleteSuccess = () => {
    setRefreshTrigger(prev => prev + 1);
  };

  return (
    <div className="app-container">
      {/* Navigation Bar */}
      <Navbar bg="primary" variant="dark" expand="lg">
        <Container>
          <Navbar.Brand href="#home">
            <i className="bi bi-people-fill me-2"></i>
            Customer Management System
          </Navbar.Brand>
          <Nav className="ms-auto">
            <Nav.Link 
              onClick={() => setCurrentView('list')} 
              active={currentView === 'list'}
            >
              Customer List
            </Nav.Link>
          </Nav>
        </Container>
      </Navbar>

      {/* Main Content */}
      <Container className="main-content">
        {currentView === 'list' ? (
          <CustomerList 
            onAddNew={handleAddNew}
            onEdit={handleEdit}
            onDeleteSuccess={handleDeleteSuccess}
            refreshTrigger={refreshTrigger}
          />
        ) : (
          <CustomerForm 
            customer={selectedCustomer}
            onSave={handleSaveSuccess}
            onCancel={handleCancel}
          />
        )}
      </Container>

      {/* Footer */}
      <footer className="bg-light text-center py-3 mt-5">
        <Container>
          <p className="mb-0 text-muted">
            &copy; {new Date().getFullYear()} Customer Management System. All rights reserved.
          </p>
        </Container>
      </footer>
    </div>
  );
}

export default App;
