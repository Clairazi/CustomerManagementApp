import React, { useState, useEffect } from 'react';
import { 
  Card, 
  Button, 
  Form, 
  Row, 
  Col, 
  Alert, 
  Spinner,
  Table
} from 'react-bootstrap';
import orderService from '../services/orderService';
import customerService from '../services/customerService';
import productService from '../services/productService';

/**
 * OrderForm Component
 * Master-Detail form for adding/editing orders.
 * Master section: Order header (customer, date, status)
 * Detail section: Dynamic table of order items (products)
 */
function OrderForm({ order, onSave, onCancel }) {
  // Form state - Master (Order Header)
  const [formData, setFormData] = useState({
    customerId: '',
    orderDate: new Date().toISOString().split('T')[0],
    status: 'Pending'
  });

  // Form state - Detail (Order Items)
  const [orderItems, setOrderItems] = useState([{
    productId: '',
    quantity: 1,
    unitPrice: 0
  }]);

  // Lookup data
  const [customers, setCustomers] = useState([]);
  const [products, setProducts] = useState([]);

  // UI state
  const [loading, setLoading] = useState(false);
  const [dataLoading, setDataLoading] = useState(true);
  const [error, setError] = useState(null);
  const [validationErrors, setValidationErrors] = useState({});

  // Status options
  const statusOptions = ['Pending', 'Processing', 'Completed', 'Cancelled'];

  /**
   * Load customers and products for dropdowns
   */
  useEffect(() => {
    const loadData = async () => {
      try {
        setDataLoading(true);
        const [customersData, productsData] = await Promise.all([
          customerService.getAllCustomers(),
          productService.getAllProducts()
        ]);
        setCustomers(customersData);
        setProducts(productsData);
      } catch (err) {
        setError('Failed to load data. Please try again.');
        console.error('Error loading data:', err);
      } finally {
        setDataLoading(false);
      }
    };

    loadData();
  }, []);

  /**
   * Initialize form with order data if editing
   */
  useEffect(() => {
    if (order) {
      setFormData({
        customerId: order.customerId?.toString() || '',
        orderDate: order.orderDate ? order.orderDate.split('T')[0] : new Date().toISOString().split('T')[0],
        status: order.status || 'Pending'
      });

      if (order.orderItems && order.orderItems.length > 0) {
        setOrderItems(order.orderItems.map(item => ({
          productId: item.productId?.toString() || '',
          quantity: item.quantity || 1,
          unitPrice: item.unitPrice || 0
        })));
      }
    }
  }, [order]);

  /**
   * Handle master form field changes
   */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    // Clear validation error for this field
    if (validationErrors[name]) {
      setValidationErrors(prev => ({ ...prev, [name]: null }));
    }
  };

  /**
   * Handle order item field changes
   */
  const handleItemChange = (index, field, value) => {
    const updatedItems = [...orderItems];
    updatedItems[index] = {
      ...updatedItems[index],
      [field]: value
    };

    // If product changed, auto-fill unit price from product
    if (field === 'productId' && value) {
      const selectedProduct = products.find(p => p.id.toString() === value);
      if (selectedProduct) {
        updatedItems[index].unitPrice = selectedProduct.price;
      }
    }

    setOrderItems(updatedItems);

    // Clear validation error for items
    if (validationErrors.orderItems) {
      setValidationErrors(prev => ({ ...prev, orderItems: null }));
    }
  };

  /**
   * Add a new order item row
   */
  const handleAddItem = () => {
    setOrderItems([...orderItems, {
      productId: '',
      quantity: 1,
      unitPrice: 0
    }]);
  };

  /**
   * Remove an order item row
   */
  const handleRemoveItem = (index) => {
    if (orderItems.length === 1) {
      setValidationErrors(prev => ({ ...prev, orderItems: 'At least one item is required' }));
      return;
    }
    setOrderItems(orderItems.filter((_, i) => i !== index));
  };

  /**
   * Calculate subtotal for an item
   */
  const calculateSubtotal = (item) => {
    return (parseFloat(item.quantity) || 0) * (parseFloat(item.unitPrice) || 0);
  };

  /**
   * Calculate total amount for all items
   */
  const calculateTotalAmount = () => {
    return orderItems.reduce((total, item) => total + calculateSubtotal(item), 0);
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
   * Validate form
   */
  const validateForm = () => {
    const errors = {};

    // Validate master form
    if (!formData.customerId) {
      errors.customerId = 'Customer is required';
    }

    if (!formData.orderDate) {
      errors.orderDate = 'Order date is required';
    }

    if (!formData.status) {
      errors.status = 'Status is required';
    }

    // Validate order items (detail form)
    if (orderItems.length === 0) {
      errors.orderItems = 'At least one order item is required';
    } else {
      const hasEmptyProduct = orderItems.some(item => !item.productId);
      const hasInvalidQuantity = orderItems.some(item => !item.quantity || parseInt(item.quantity) < 1);
      const hasInvalidPrice = orderItems.some(item => !item.unitPrice || parseFloat(item.unitPrice) <= 0);

      if (hasEmptyProduct) {
        errors.orderItems = 'All order items must have a product selected';
      } else if (hasInvalidQuantity) {
        errors.orderItems = 'All order items must have a quantity of at least 1';
      } else if (hasInvalidPrice) {
        errors.orderItems = 'All order items must have a valid unit price';
      }
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  /**
   * Handle form submission
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);

    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);

      // Prepare data for API
      const orderData = {
        customerId: parseInt(formData.customerId),
        orderDate: formData.orderDate,
        status: formData.status,
        orderItems: orderItems.map(item => ({
          productId: parseInt(item.productId),
          quantity: parseInt(item.quantity),
          unitPrice: parseFloat(item.unitPrice)
        }))
      };

      if (order) {
        // Update existing order
        await orderService.updateOrder(order.id, orderData);
      } else {
        // Create new order
        await orderService.createOrder(orderData);
      }

      onSave();
    } catch (err) {
      const errorMessage = err.response?.data?.message || 'An error occurred while saving the order';
      setError(errorMessage);
      console.error('Error saving order:', err);
    } finally {
      setLoading(false);
    }
  };

  if (dataLoading) {
    return (
      <div className="text-center py-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  return (
    <div>
      <h2 className="mb-4">{order ? 'Edit Order' : 'Add New Order'}</h2>

      {error && (
        <Alert variant="danger" dismissible onClose={() => setError(null)}>
          {error}
        </Alert>
      )}

      <Form onSubmit={handleSubmit}>
        {/* Master Section - Order Header */}
        <Card className="mb-4">
          <Card.Header>
            <h5 className="mb-0">Order Information</h5>
          </Card.Header>
          <Card.Body>
            <Row>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Customer <span className="text-danger">*</span></Form.Label>
                  <Form.Select
                    name="customerId"
                    value={formData.customerId}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.customerId}
                  >
                    <option value="">Select a customer...</option>
                    {customers.map(customer => (
                      <option key={customer.id} value={customer.id}>
                        {customer.firstName} {customer.lastName}
                      </option>
                    ))}
                  </Form.Select>
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.customerId}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Order Date <span className="text-danger">*</span></Form.Label>
                  <Form.Control
                    type="date"
                    name="orderDate"
                    value={formData.orderDate}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.orderDate}
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.orderDate}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>
              <Col md={4}>
                <Form.Group className="mb-3">
                  <Form.Label>Status <span className="text-danger">*</span></Form.Label>
                  <Form.Select
                    name="status"
                    value={formData.status}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.status}
                  >
                    {statusOptions.map(status => (
                      <option key={status} value={status}>
                        {status}
                      </option>
                    ))}
                  </Form.Select>
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.status}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>
            </Row>
          </Card.Body>
        </Card>

        {/* Detail Section - Order Items */}
        <Card className="mb-4">
          <Card.Header className="d-flex justify-content-between align-items-center">
            <h5 className="mb-0">Order Items</h5>
            <Button variant="outline-primary" size="sm" onClick={handleAddItem}>
              <i className="bi bi-plus-circle me-1"></i>
              Add Item
            </Button>
          </Card.Header>
          <Card.Body>
            {validationErrors.orderItems && (
              <Alert variant="danger" className="mb-3">
                {validationErrors.orderItems}
              </Alert>
            )}

            <div className="table-responsive">
              <Table bordered>
                <thead>
                  <tr>
                    <th style={{width: '35%'}}>Product <span className="text-danger">*</span></th>
                    <th style={{width: '15%'}}>Quantity <span className="text-danger">*</span></th>
                    <th style={{width: '20%'}}>Unit Price <span className="text-danger">*</span></th>
                    <th style={{width: '20%'}}>Subtotal</th>
                    <th style={{width: '10%'}}>Action</th>
                  </tr>
                </thead>
                <tbody>
                  {orderItems.map((item, index) => (
                    <tr key={index}>
                      <td>
                        <Form.Select
                          value={item.productId}
                          onChange={(e) => handleItemChange(index, 'productId', e.target.value)}
                        >
                          <option value="">Select a product...</option>
                          {products.map(product => (
                            <option key={product.id} value={product.id}>
                              {product.name} ({formatPrice(product.price)})
                            </option>
                          ))}
                        </Form.Select>
                      </td>
                      <td>
                        <Form.Control
                          type="number"
                          min="1"
                          value={item.quantity}
                          onChange={(e) => handleItemChange(index, 'quantity', e.target.value)}
                        />
                      </td>
                      <td>
                        <Form.Control
                          type="number"
                          step="0.01"
                          min="0.01"
                          value={item.unitPrice}
                          onChange={(e) => handleItemChange(index, 'unitPrice', e.target.value)}
                        />
                      </td>
                      <td className="text-end align-middle">
                        <strong>{formatPrice(calculateSubtotal(item))}</strong>
                      </td>
                      <td className="text-center">
                        <Button
                          variant="outline-danger"
                          size="sm"
                          onClick={() => handleRemoveItem(index)}
                          disabled={orderItems.length === 1}
                        >
                          <i className="bi bi-trash"></i>
                        </Button>
                      </td>
                    </tr>
                  ))}
                </tbody>
                <tfoot>
                  <tr>
                    <td colSpan="3" className="text-end"><strong>Total Amount:</strong></td>
                    <td className="text-end">
                      <strong className="text-primary fs-5">{formatPrice(calculateTotalAmount())}</strong>
                    </td>
                    <td></td>
                  </tr>
                </tfoot>
              </Table>
            </div>
          </Card.Body>
        </Card>

        {/* Form Actions */}
        <div className="d-flex gap-2">
          <Button variant="primary" type="submit" disabled={loading}>
            {loading ? (
              <>
                <Spinner as="span" animation="border" size="sm" role="status" aria-hidden="true" className="me-2" />
                Saving...
              </>
            ) : (
              <>
                <i className="bi bi-check-circle me-2"></i>
                {order ? 'Update Order' : 'Create Order'}
              </>
            )}
          </Button>
          <Button variant="secondary" onClick={onCancel} disabled={loading}>
            <i className="bi bi-x-circle me-2"></i>
            Cancel
          </Button>
        </div>
      </Form>
    </div>
  );
}

export default OrderForm;
