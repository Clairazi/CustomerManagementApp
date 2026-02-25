import React, { useState } from 'react';
import { Container, Card, Form, Button, Alert, Spinner } from 'react-bootstrap';
import { useAuth } from '../context/AuthContext';

/**
 * Login Component
 * Provides user login form with username and password fields
 */
function Login({ onRegisterClick }) {
  const { login } = useAuth();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  /**
   * Handle form submission
   */
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    // Basic validation
    if (!username.trim() || !password.trim()) {
      setError('Please enter both username and password');
      setLoading(false);
      return;
    }

    const result = await login(username, password);
    
    if (!result.success) {
      setError(result.message);
    }
    
    setLoading(false);
  };

  return (
    <div className="login-page d-flex align-items-center justify-content-center" style={{ minHeight: '100vh', backgroundColor: '#f5f5f5' }}>
      <Container style={{ maxWidth: '400px' }}>
        <Card className="shadow">
          <Card.Header className="bg-primary text-white text-center py-3">
            <h4 className="mb-0">
              <i className="bi bi-building me-2"></i>
              Customer Management System
            </h4>
          </Card.Header>
          <Card.Body className="p-4">
            <h5 className="text-center mb-4">Sign In</h5>
            
            {error && (
              <Alert variant="danger" dismissible onClose={() => setError('')}>
                <i className="bi bi-exclamation-triangle me-2"></i>
                {error}
              </Alert>
            )}

            <Form onSubmit={handleSubmit}>
              <Form.Group className="mb-3" controlId="username">
                <Form.Label>
                  <i className="bi bi-person me-2"></i>
                  Username
                </Form.Label>
                <Form.Control
                  type="text"
                  placeholder="Enter username"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                  disabled={loading}
                  autoComplete="username"
                  autoFocus
                />
              </Form.Group>

              <Form.Group className="mb-4" controlId="password">
                <Form.Label>
                  <i className="bi bi-lock me-2"></i>
                  Password
                </Form.Label>
                <Form.Control
                  type="password"
                  placeholder="Enter password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  disabled={loading}
                  autoComplete="current-password"
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
                    Signing in...
                  </>
                ) : (
                  <>
                    <i className="bi bi-box-arrow-in-right me-2"></i>
                    Sign In
                  </>
                )}
              </Button>
            </Form>

            <hr className="my-4" />

            <div className="text-center">
              <p className="text-muted mb-2">Don't have an account?</p>
              <Button 
                variant="outline-secondary" 
                onClick={onRegisterClick}
                disabled={loading}
                className="w-100"
              >
                <i className="bi bi-person-plus me-2"></i>
                Create Account
              </Button>
            </div>

            <hr className="my-4" />

            <div className="text-center text-muted small">
              <p className="mb-1"><strong>Test Credentials:</strong></p>
              <p className="mb-0">admin / admin123</p>
              <p className="mb-0">user / user123</p>
            </div>
          </Card.Body>
        </Card>
        
        <p className="text-center text-muted mt-3 small">
          &copy; {new Date().getFullYear()} Customer Management System
        </p>
      </Container>
    </div>
  );
}

export default Login;
