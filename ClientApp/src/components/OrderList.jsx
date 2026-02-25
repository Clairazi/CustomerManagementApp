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
  Modal,
  Badge
} from 'react-bootstrap';
import orderService from '../services/orderService';
import customerService from '../services/customerService';

/**
 * OrderList Component
 * Displays a list of orders with filtering, search, and action buttons.
 * Shows Order ID, Customer Name, Order Date, Total Amount, and Status.
 */
function OrderList({ onAddNew, onEdit, onDeleteSuccess, refreshTrigger }) {
  // State management
  const [orders, setOrders] = useState([]);
  const [customers, setCustomers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);
  const [orderToDelete, setOrderToDelete] = useState(null);

  // Filter state
  const [filters, setFilters] = useState({
    orderId: '',
    dateFrom: '',
    dateTo: '',
    customerId: ''
  });

  /**
   * Load orders from API
   */
  const loadOrders = async () => {
    try {
      setLoading(true);
      setError(null);
      const data = await orderService.getAllOrders(filters);
      setOrders(data);
    } catch (err) {
      setError('Failed to load orders. Please try again.');
      console.error('Error loading orders:', err);
    } finally {
      setLoading(false);
    }
  };

  /**
   * Load customers for the filter dropdown
   */
  const loadCustomers = async () => {
    try {
      const data = await customerService.getAllCustomers();
      setCustomers(data);
    } catch (err) {
      console.error('Error loading customers for filter:', err);
    }
  };

  /**
   * Effect hook to load data on component mount and when refreshTrigger changes
   */
  useEffect(() => {
    loadOrders();
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
    loadOrders();
  };

  /**
   * Handle clear filters
   */
  const handleClearFilters = () => {
    setFilters({
      orderId: '',
      dateFrom: '',
      dateTo: '',
      customerId: ''
    });
    // Reload orders without filters
    setTimeout(() => {
      orderService.getAllOrders({})
        .then(data => setOrders(data))
        .catch(err => setError('Failed to load orders'));
    }, 100);
  };

  /**
   * Show delete confirmation modal
   */
  const showDeleteConfirmation = (order) => {
    setOrderToDelete(order);
    setShowDeleteModal(true);
  };

  /**
   * Handle delete order
   */
  const handleDelete = async () => {
    if (!orderToDelete) return;

    try {
      setError(null);
      await orderService.deleteOrder(orderToDelete.id);
      setSuccess(`Order #${orderToDelete.id} deleted successfully!`);
      setTimeout(() => setSuccess(null), 3000);
      setShowDeleteModal(false);
      setOrderToDelete(null);
      onDeleteSuccess();
    } catch (err) {
      setError('Failed to delete order. Please try again.');
      console.error('Error deleting order:', err);
      setShowDeleteModal(false);
    }
  };

  /**
   * Close delete modal
   */
  const handleCloseDeleteModal = () => {
    setShowDeleteModal(false);
    setOrderToDelete(null);
  };

  /**
   * Format price for display
   */
  const formatPrice = (price) => {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(price);
  };

  /**
   * Format date for display
   */
  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  /**
   * Get status badge variant
   */
  const getStatusBadge = (status) => {
    const variants = {
      'Pending': 'warning',
      'Processing': 'info',
      'Completed': 'success',
      'Cancelled': 'danger'
    };
    return variants[status] || 'secondary';
  };

  return (
    <div>
      <h2 className="mb-4">Order List</h2>

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
                  <Form.Label>Order ID</Form.Label>
                  <Form.Control
                    type="number"
                    name="orderId"
                    placeholder="Enter order ID"
                    value={filters.orderId}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Date From</Form.Label>
                  <Form.Control
                    type="date"
                    name="dateFrom"
                    value={filters.dateFrom}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Date To</Form.Label>
                  <Form.Control
                    type="date"
                    name="dateTo"
                    value={filters.dateTo}
                    onChange={handleFilterChange}
                  />
                </Form.Group>
              </Col>
              <Col md={3}>
                <Form.Group className="mb-3">
                  <Form.Label>Customer</Form.Label>
                  <Form.Select
                    name="customerId"
                    value={filters.customerId}
                    onChange={handleFilterChange}
                  >
                    <option value="">All Customers</option>
                    {customers.map(customer => (
                      <option key={customer.id} value={customer.id}>
                        {customer.firstName} {customer.lastName}
                      </option>
                    ))}
                  </Form.Select>
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
          Add New Order
        </Button>
      </div>

      {/* Order Table */}
      <Card>
        <Card.Body>
          {loading ? (
            <div className="loading-spinner">
              <Spinner animation="border" role="status">
                <span className="visually-hidden">Loading...</span>
              </Spinner>
            </div>
          ) : orders.length === 0 ? (
            <Alert variant="info">
              No orders found. {filters.orderId || filters.dateFrom || filters.dateTo || filters.customerId 
                ? 'Try adjusting your filters.' 
                : 'Click "Add New Order" to get started.'}
            </Alert>
          ) : (
            <div className="table-responsive">
              <Table striped bordered hover>
                <thead>
                  <tr>
                    <th>Order ID</th>
                    <th>Customer</th>
                    <th>Order Date</th>
                    <th>Total Amount</th>
                    <th>Status</th>
                    <th>Items</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {orders.map((order) => (
                    <tr key={order.id}>
                      <td>#{order.id}</td>
                      <td>{order.customerName}</td>
                      <td>{formatDate(order.orderDate)}</td>
                      <td>{formatPrice(order.totalAmount)}</td>
                      <td>
                        <Badge bg={getStatusBadge(order.status)}>
                          {order.status}
                        </Badge>
                      </td>
                      <td>{order.orderItems?.length || 0}</td>
                      <td>
                        <Button
                          variant="warning"
                          size="sm"
                          className="btn-action"
                          onClick={() => onEdit(order)}
                        >
                          <i className="bi bi-pencil me-1"></i>
                          Edit
                        </Button>
                        <Button
                          variant="danger"
                          size="sm"
                          onClick={() => showDeleteConfirmation(order)}
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
                Total Orders: {orders.length}
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
          Are you sure you want to delete order{' '}
          <strong>#{orderToDelete?.id}</strong> for customer{' '}
          <strong>{orderToDelete?.customerName}</strong>?
          <br /><br />
          <small className="text-muted">This will also delete all order items. This action cannot be undone.</small>
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

export default OrderList;
