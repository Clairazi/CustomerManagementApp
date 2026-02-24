import React, { useState, useEffect } from 'react';
import { Card, Form, Button, Alert, Spinner, Row, Col } from 'react-bootstrap';
import customerService from '../services/customerService';

/**
 * CustomerForm Component
 * Form for adding new customers or editing existing ones
 */
function CustomerForm({ customer, onSave, onCancel }) {
  // Form state
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: ''
  });

  // UI state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [validationErrors, setValidationErrors] = useState({});

  // Determine if we're editing or creating
  const isEditMode = customer !== null;

  /**
   * Load customer data when editing
   */
  useEffect(() => {
    if (customer) {
      setFormData({
        firstName: customer.firstName || '',
        lastName: customer.lastName || '',
        email: customer.email || '',
        phoneNumber: customer.phoneNumber || ''
      });
    }
  }, [customer]);

  /**
   * Handle form input changes
   */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
    
    // Clear validation error for this field
    if (validationErrors[name]) {
      setValidationErrors(prev => ({
        ...prev,
        [name]: null
      }));
    }
  };

  /**
   * Validate form data
   * @returns {boolean} True if valid, false otherwise
   */
  const validateForm = () => {
    const errors = {};

    // First Name validation (required)
    if (!formData.firstName.trim()) {
      errors.firstName = 'First name is required';
    } else if (formData.firstName.length > 100) {
      errors.firstName = 'First name cannot exceed 100 characters';
    }

    // Last Name validation (required)
    if (!formData.lastName.trim()) {
      errors.lastName = 'Last name is required';
    } else if (formData.lastName.length > 100) {
      errors.lastName = 'Last name cannot exceed 100 characters';
    }

    // Email validation (optional, but must be valid if provided)
    if (formData.email && formData.email.trim()) {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(formData.email)) {
        errors.email = 'Please enter a valid email address';
      } else if (formData.email.length > 200) {
        errors.email = 'Email cannot exceed 200 characters';
      }
    }

    // Phone Number validation (optional)
    if (formData.phoneNumber && formData.phoneNumber.length > 20) {
      errors.phoneNumber = 'Phone number cannot exceed 20 characters';
    }

    setValidationErrors(errors);
    return Object.keys(errors).length === 0;
  };

  /**
   * Handle form submission
   */
  const handleSubmit = async (e) => {
    e.preventDefault();

    // Validate form
    if (!validateForm()) {
      return;
    }

    try {
      setLoading(true);
      setError(null);

      // Prepare data for submission
      const customerData = {
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim() || null,
        phoneNumber: formData.phoneNumber.trim() || null
      };

      if (isEditMode) {
        // Update existing customer
        await customerService.updateCustomer(customer.id, customerData);
      } else {
        // Create new customer
        await customerService.createCustomer(customerData);
      }

      // Success - call parent callback
      onSave();
    } catch (err) {
      // Handle errors
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else {
        setError(`Failed to ${isEditMode ? 'update' : 'create'} customer. Please try again.`);
      }
      console.error('Error saving customer:', err);
    } finally {
      setLoading(false);
    }
  };

  /**
   * Handle cancel button
   */
  const handleCancelClick = () => {
    onCancel();
  };

  return (
    <div>
      <h2 className="mb-4">{isEditMode ? 'Edit Customer' : 'Add New Customer'}</h2>

      <Card>
        <Card.Body>
          {/* Error Alert */}
          {error && (
            <Alert variant="danger" dismissible onClose={() => setError(null)}>
              {error}
            </Alert>
          )}

          <Form onSubmit={handleSubmit}>
            <Row>
              <Col md={6}>
                {/* First Name */}
                <Form.Group className="mb-3">
                  <Form.Label className="form-label-required">First Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="firstName"
                    placeholder="Enter first name"
                    value={formData.firstName}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.firstName}
                    disabled={loading}
                    required
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.firstName}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>

              <Col md={6}>
                {/* Last Name */}
                <Form.Group className="mb-3">
                  <Form.Label className="form-label-required">Last Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="lastName"
                    placeholder="Enter last name"
                    value={formData.lastName}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.lastName}
                    disabled={loading}
                    required
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.lastName}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>
            </Row>

            <Row>
              <Col md={6}>
                {/* Email */}
                <Form.Group className="mb-3">
                  <Form.Label>Email</Form.Label>
                  <Form.Control
                    type="email"
                    name="email"
                    placeholder="Enter email address"
                    value={formData.email}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.email}
                    disabled={loading}
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.email}
                  </Form.Control.Feedback>
                  <Form.Text className="text-muted">
                    Optional
                  </Form.Text>
                </Form.Group>
              </Col>

              <Col md={6}>
                {/* Phone Number */}
                <Form.Group className="mb-3">
                  <Form.Label>Phone Number</Form.Label>
                  <Form.Control
                    type="tel"
                    name="phoneNumber"
                    placeholder="Enter phone number"
                    value={formData.phoneNumber}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.phoneNumber}
                    disabled={loading}
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.phoneNumber}
                  </Form.Control.Feedback>
                  <Form.Text className="text-muted">
                    Optional
                  </Form.Text>
                </Form.Group>
              </Col>
            </Row>

            {/* Form Actions */}
            <div className="d-flex gap-2 mt-4">
              <Button 
                variant="primary" 
                type="submit" 
                disabled={loading}
              >
                {loading ? (
                  <>
                    <Spinner
                      as="span"
                      animation="border"
                      size="sm"
                      role="status"
                      aria-hidden="true"
                      className="me-2"
                    />
                    {isEditMode ? 'Updating...' : 'Creating...'}
                  </>
                ) : (
                  <>
                    <i className="bi bi-check-circle me-2"></i>
                    {isEditMode ? 'Update Customer' : 'Create Customer'}
                  </>
                )}
              </Button>
              <Button 
                variant="secondary" 
                onClick={handleCancelClick}
                disabled={loading}
              >
                <i className="bi bi-x-circle me-2"></i>
                Cancel
              </Button>
            </div>
          </Form>
        </Card.Body>
      </Card>

      {/* Help Text */}
      <Card className="mt-3">
        <Card.Body>
          <h6>Form Instructions:</h6>
          <ul className="mb-0">
            <li><strong>First Name</strong> and <strong>Last Name</strong> are required fields (marked with *)</li>
            <li><strong>Email</strong> and <strong>Phone Number</strong> are optional</li>
            <li>If you provide an email, it must be in a valid format (e.g., user@example.com)</li>
            <li>All fields have maximum character limits to ensure data quality</li>
          </ul>
        </Card.Body>
      </Card>
    </div>
  );
}

export default CustomerForm;
