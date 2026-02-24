import React, { useState, useEffect } from 'react';
import { 
  Card, 
  Button, 
  Table, 
  Form, 
  Row, 
  Col, 
  Alert, 
  Spinner,
  Modal 
} from 'react-bootstrap';
import customerService from '../services/customerService';

/**
 * CustomerList Component
 * Displays a list of customers with filtering, search, and action buttons
 */
function CustomerList({ onAddNew, onEdit, onDeleteSuccess, refreshTrigger }) {
  // State management
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [customerToDelete, setCustomerToDelete] = useState(null);

  // Filter state
  const [filters, setFilters] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: ''
  });

  /**
   * Load customers from API
   */
  const loadCustomers = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await customerService.getAllCustomers(filters);
      setCustomers(data);
    } catch (err) {
      setError('Failed to load customers. Please try again.');
      console.error('Error loading customers:', err);
    } finally {
      setLoading(false);
    }
  };

  /**
   * Effect hook to load customers on component mount and when filters change
   */
  useEffect(() => {
    loadCustomers();
  }, [refreshTrigger]);

  /**
   * Handle filter input changes
   */
  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters(prev => ({
      ...prev,
      [name]: value
    }));
  };

  /**
   * Handle search button click
   */
  const handleSearch = () => {
    loadCustomers();
  };

  /**
   * Handle clear filters
   */
  const handleClearFilters = () => {
    setFilters({
      firstName: '',
      lastName: '',
      email: '',
      phoneNumber: ''
    });
    // Reload customers without filters
    setTimeout(() => {
      customerService.getAllCustomers({})
        .then(data => setCustomers(data))
        .catch(err => setError('Failed to load customers'));
    }, 100);
  };

  /**
   * Handle export to Excel
   */
  const handleExport = async () => {
    try {
      setError(null);
      await customerService.exportCustomersToExcel(filters);
      setSuccess('Customers exported successfully!');
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Failed to export customers. Please try again.');
      console.error('Error exporting customers:', err);
    }
  };

  /**
   * Show delete confirmation modal
   */
  const showDeleteConfirmation = (customer) => {
    setCustomerToDelete(customer);
    setShowDeleteModal(true);
  };

  /**
   * Handle delete customer
   */
  const handleDelete = async () => {
    if (!customerToDelete) return;

    try {
      setError(null);
      await customerService.deleteCustomer(customerToDelete.id);
      setSuccess(`Customer "${customerToDelete.firstName} ${customerToDelete.lastName}" deleted successfully!`);
      setTimeout(() => setSuccess(null), 3000);
      setShowDeleteModal(false);
      setCustomerToDelete(null);
      onDeleteSuccess();
    } catch (err) {
      setError('Failed to delete customer. Please try again.');
      console.error('Error deleting customer:', err);
      setShowDeleteModal(false);
    }
  };

  /**
   * Close delete modal
   */
  const handleCloseDeleteModal = () => {
    setShowDeleteModal(false);
    setCustomerToDelete(null);
  };

  return (
    <div>
      <h2 className="mb-4">Customer List</h2>

      {/* Success Alert */}
      {success && (
        <Alert variant="success" dismissible onClose={() => setSuccess(null)}>
          {success}
        </Alert>
      )}

      {/* Error Alert */}
      {error && (
        <Alert variant="danger" dismissible onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      {/* Filter Section */}
      <Card className="mb-4">
        <Card.Header>
          <h5 className="mb-0">Search & Filter</h5>
        </Card.Header>
        <Card.Body>
          <Form>
            <Row>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>First Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="firstName"
                    placeholder="Enter first name"
                    value={filters.firstName}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Last Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="lastName"
                    placeholder="Enter last name"
                    value={filters.lastName}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="text"
                    name="email"
                    placeholder="Enter email"
                    value={filters.email}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Phone Number</Form.Label>
                  <Form.Control
                    type="text"
                    name="phoneNumber"
                    placeholder="Enter phone number"
                    value={filters.phoneNumber}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
            </Row>
            <div className="d-flex gap-2">
              <Button variant="primary" onClick={handleSearch}>
                <i className="bi bi-search me-2"></i>
                Search
              </Button>
              <Button variant="secondary" onClick={handleClearFilters}>
                <i className="bi bi-x-circle me-2"></i>
                Clear Filters
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>

      {/* Action Buttons */}
      <div className="action-buttons d-flex gap-2 mb-3">
        <Button variant="success" onClick={onAddNew}>
          <i className="bi bi-plus-circle me-2"></i>
          Add New Customer
        </Button>
        <Button variant="info" onClick={handleExport}>
          <i className="bi bi-file-excel me-2"></i>
          Export to Excel
        </Button>
      </div>

      {/* Customer Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="loading-spinner">
              <Spinner animation="border" role="status">
                <span className="visually-hidden">Loading...</span>
              </Spinner>
            </div>
          ) : customers.length === 0 ? (
            <Alert variant="info">
              No customers found. {filters.firstName || filters.lastName || filters.email || filters.phoneNumber 
                ? 'Try adjusting your filters.' 
                : 'Click "Add New Customer" to get started.'}
            </Alert>
          ) : (
            <div className="table-responsive">
              <Table striped bordered hover>
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>First Name</th>
                    <th>Last Name</th>
                    <th>Email</th>
                    <th>Phone Number</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {customers.map((customer) => (
                    <tr key={customer.id}>
                      <td>{customer.id}</td>
                      <td>{customer.firstName}</td>
                      <td>{customer.lastName}</td>
                      <td>{customer.email || '-'}</td>
                      <td>{customer.phoneNumber || '-'}</td>
                      <td>
                        <Button
                          variant="warning"
                          size="sm"
                          className="btn-action"
                          onClick={() => onEdit(customer)}
                        >
                          <i className="bi bi-pencil me-1"></i>
                          Edit
                        </Button>
                        <Button
                          variant="danger"
                          size="sm"
                          onClick={() => showDeleteConfirmation(customer)}
                        >
                          <i className="bi bi-trash me-1"></i>
                          Delete
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </Table>
              <div className="text-muted">
                Total Customers: {customers.length}
              </div>
            </div>
          )}
        </Card.Body>
      </Card>

      {/* Delete Confirmation Modal */}
      <Modal show={showDeleteModal} onHide={handleCloseDeleteModal}>
        <Modal.Header closeButton>
          <Modal.Title>Confirm Delete</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          Are you sure you want to delete customer{' '}
          <strong>
            {customerToDelete?.firstName} {customerToDelete?.lastName}
          </strong>
          ? This action cannot be undone.
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleCloseDeleteModal}>
            Cancel
          </Button>
          <Button variant="danger" onClick={handleDelete}>
            Delete
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
}

export default CustomerList;
