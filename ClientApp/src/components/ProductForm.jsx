import React, { useState, useEffect } from 'react';
import { Card, Form, Button, Alert, Spinner, Row, Col } from 'react-bootstrap';
import productService from '../services/productService';

/**
 * ProductForm Component
 * Form for adding new products or editing existing ones
 */
function ProductForm({ product, onSave, onCancel }) {
  // Form state
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    sku: ''
  });

  // UI state
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [validationErrors, setValidationErrors] = useState({});

  // Determine if we're editing or creating
  const isEditMode = product !== null;

  /**
   * Load product data when editing
   */
  useEffect(() => {
    if (product) {
      setFormData({
        name: product.name || '',
        description: product.description || '',
        price: product.price !== undefined ? product.price.toString() : '',
        sku: product.sku || ''
      });
    }
  }, [product]);

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

    // Name validation (required)
    if (!formData.name.trim()) {
      errors.name = 'Product name is required';
    } else if (formData.name.length > 200) {
      errors.name = 'Product name cannot exceed 200 characters';
    }

    // Description validation (optional)
    if (formData.description && formData.description.length > 1000) {
      errors.description = 'Description cannot exceed 1000 characters';
    }

    // Price validation
    if (formData.price && isNaN(parseFloat(formData.price))) {
      errors.price = 'Please enter a valid price';
    } else if (formData.price && parseFloat(formData.price) < 0) {
      errors.price = 'Price cannot be negative';
    }

    // SKU validation (optional)
    if (formData.sku && formData.sku.length > 50) {
      errors.sku = 'SKU cannot exceed 50 characters';
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
      const productData = {
        name: formData.name.trim(),
        description: formData.description.trim() || null,
        price: parseFloat(formData.price) || 0,
        sku: formData.sku.trim() || null
      };

      if (isEditMode) {
        // Update existing product
        await productService.updateProduct(product.id, productData);
      } else {
        // Create new product
        await productService.createProduct(productData);
      }

      // Success - call parent callback
      onSave();
    } catch (err) {
      // Handle errors
      if (err.response && err.response.data && err.response.data.message) {
        setError(err.response.data.message);
      } else {
        setError(`Failed to ${isEditMode ? 'update' : 'create'} product. Please try again.`);
      }
      console.error('Error saving product:', err);
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
      <h2 className="mb-4">{isEditMode ? 'Edit Product' : 'Add New Product'}</h2>

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
                {/* Product Name */}
                <Form.Group className="mb-3">
                  <Form.Label className="form-label-required">Product Name</Form.Label>
                  <Form.Control
                    type="text"
                    name="name"
                    placeholder="Enter product name"
                    value={formData.name}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.name}
                    disabled={loading}
                    required
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.name}
                  </Form.Control.Feedback>
                </Form.Group>
              </Col>

              <Col md={6}>
                {/* SKU */}
                <Form.Group className="mb-3">
                  <Form.Label>SKU</Form.Label>
                  <Form.Control
                    type="text"
                    name="sku"
                    placeholder="Enter SKU"
                    value={formData.sku}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.sku}
                    disabled={loading}
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.sku}
                  </Form.Control.Feedback>
                  <Form.Text className="text-muted">
                    Optional - Stock Keeping Unit
                  </Form.Text>
                </Form.Group>
              </Col>
            </Row>

            <Row>
              <Col md={6}>
                {/* Price */}
                <Form.Group className="mb-3">
                  <Form.Label>Price</Form.Label>
                  <Form.Control
                    type="number"
                    name="price"
                    placeholder="Enter price"
                    value={formData.price}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.price}
                    disabled={loading}
                    step="0.01"
                    min="0"
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.price}
                  </Form.Control.Feedback>
                  <Form.Text className="text-muted">
                    Optional
                  </Form.Text>
                </Form.Group>
              </Col>
            </Row>

            <Row>
              <Col md={12}>
                {/* Description */}
                <Form.Group className="mb-3">
                  <Form.Label>Description</Form.Label>
                  <Form.Control
                    as="textarea"
                    rows={3}
                    name="description"
                    placeholder="Enter product description"
                    value={formData.description}
                    onChange={handleChange}
                    isInvalid={!!validationErrors.description}
                    disabled={loading}
                  />
                  <Form.Control.Feedback type="invalid">
                    {validationErrors.description}
                  </Form.Control.Feedback>
                  <Form.Text className="text-muted">
                    Optional - Maximum 1000 characters
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
                    {isEditMode ? 'Update Product' : 'Create Product'}
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
            <li><strong>Product Name</strong> is a required field (marked with *)</li>
            <li><strong>SKU</strong>, <strong>Price</strong>, and <strong>Description</strong> are optional</li>
            <li>Price must be a positive number</li>
            <li>All fields have maximum character limits to ensure data quality</li>
          </ul>
        </Card.Body>
      </Card>
    </div>
  );
}

export default ProductForm;
