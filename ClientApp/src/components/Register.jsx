import React, { useState } from 'react';
import { Container, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';

/**
 * Registration Component
 * Provides user registration form
 */
function Register({ onLoginClick, onSuccess }) {
  const { register } = useAuth();
  const [formData, setFormData] = useState({
    username: '',
    password: '',
    confirmPassword: '',
    email: '',
    fullName: ''
  });
  const [errors, setErrors] = useState({});
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  /**
   * Handle input changes
   */
  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    // Clear field error when user types
    if (errors[name]) {
      setErrors(prev => ({ ...prev, [name]: '' }));
    }
  };

  /**
   * Validate form data
   */
  const validateForm = () => {
    const newErrors = {};

    if (!formData.username.trim()) {
      newErrors.username = 'Username is required';
    } else if (formData.username.length < 3) {
      newErrors.username = 'Username must be at least 3 characters';
    }

    if (!formData.password) {
      newErrors.password = 'Password is required';
    } else if (formData.password.length < 6) {
      newErrors.password = 'Password must be at least 6 characters';
    }

    if (formData.password !== formData.confirmPassword) {
      newErrors.confirmPassword = 'Passwords do not match';
    }

    if (formData.email && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(formData.email)) {
      newErrors.email = 'Invalid email format';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  /**
   * Handle form submission
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess('');

    if (!validateForm()) {
      return;
    }

    setLoading(true);

    const result = await register({
      username: formData.username,
      password: formData.password,
      email: formData.email || null,
      fullName: formData.fullName || null
    });

    if (result.success) {
      setSuccess('Registration successful! You can now sign in.');
      setTimeout(() => {
        onSuccess && onSuccess();
        onLoginClick();
      }, 2000);
    } else {
      setError(result.message);
    }

    setLoading(false);
  };

  return (
    <div className="register-page d-flex align-items-center justify-content-center" style={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
      <Container style={{ maxWidth: '450px' }}>
        <Card className="shadow">
          <Card.Header className="bg-primary text-white text-center py-3">
            <h4 className="mb-0">
              <i className="bi bi-person-plus me-2"></i>
              Create Account
            </h4>
          </Card.Header>
          <Card.Body className="p-4">
            {error && (
              <Alert variant="danger" dismissible onClose={() => setError('')}>
                <i className="bi bi-exclamation-triangle me-2"></i>
                {error}
              </Alert>
            )}

            {success && (
              <Alert variant="success">
                <i className="bi bi-check-circle me-2"></i>
                {success}
              </Alert>
            )}

            <Form onSubmit={handleSubmit}>
              <Form.Group className="mb-3" controlId="username">
                <Form.Label>Username *</Form.Label>
                <Form.Control
                  type="text"
                  name="username"
                  placeholder="Choose a username"
                  value={formData.username}
                  onChange={handleChange}
                  isInvalid={!!errors.username}
                  disabled={loading}
                  autoComplete="username"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.username}
                </Form.Control.Feedback>
              </Form.Group>

              <Form.Group className="mb-3" controlId="password">
                <Form.Label>Password *</Form.Label>
                <Form.Control
                  type="password"
                  name="password"
                  placeholder="Create a password"
                  value={formData.password}
                  onChange={handleChange}
                  isInvalid={!!errors.password}
                  disabled={loading}
                  autoComplete="new-password"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.password}
                </Form.Control.Feedback>
              </Form.Group>

              <Form.Group className="mb-3" controlId="confirmPassword">
                <Form.Label>Confirm Password *</Form.Label>
                <Form.Control
                  type="password"
                  name="confirmPassword"
                  placeholder="Confirm your password"
                  value={formData.confirmPassword}
                  onChange={handleChange}
                  isInvalid={!!errors.confirmPassword}
                  disabled={loading}
                  autoComplete="new-password"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.confirmPassword}
                </Form.Control.Feedback>
              </Form.Group>

              <Form.Group className="mb-3" controlId="email">
                <Form.Label>Email (Optional)</Form.Label>
                <Form.Control
                  type="email"
                  name="email"
                  placeholder="Enter your email"
                  value={formData.email}
                  onChange={handleChange}
                  isInvalid={!!errors.email}
                  disabled={loading}
                  autoComplete="email"
                />
                <Form.Control.Feedback type="invalid">
                  {errors.email}
                </Form.Control.Feedback>
              </Form.Group>

              <Form.Group className="mb-4" controlId="fullName">
                <Form.Label>Full Name (Optional)</Form.Label>
                <Form.Control
                  type="text"
                  name="fullName"
                  placeholder="Enter your full name"
                  value={formData.fullName}
                  onChange={handleChange}
                  disabled={loading}
                  autoComplete="name"
                />
              </Form.Group>

              <Button
                variant="primary"
                type="submit"
                className="w-100 py-2"
                disabled={loading}
              >
                {loading ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Creating Account...
                  </>
                ) : (
                  <>
                    <i className="bi bi-person-check me-2"></i>
                    Create Account
                  </>
                )}
              </Button>
            </Form>

            <hr className="my-4" />

            <div className="text-center">
              <p className="text-muted mb-2">Already have an account?</p>
              <Button
                variant="outline-secondary"
                onClick={onLoginClick}
                disabled={loading}
                className="w-100"
              >
                <i className="bi bi-box-arrow-in-right me-2"></i>
                Sign In
              </Button>
            </div>
          </Card.Body>
        </Card>
      </Container>
    </div>
  );
}

export default Register;
