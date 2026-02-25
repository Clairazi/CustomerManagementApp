import React from 'react';
import { Nav, Container } from 'react-bootstrap';

/**
 * Navigation Component
 * Provides navigation between Customers and Products views
 */
function Navigation({ currentModule, onModuleChange }) {
  return (
    <Container className="mt-3">
      <Nav variant="tabs" activeKey={currentModule} onSelect={onModuleChange}>
        <Nav.Item>
          <Nav.Link eventKey="customers">
            <i className="bi bi-people-fill me-2"></i>
            Customers
          </Nav.Link>
        </Nav.Item>
        <Nav.Item>
          <Nav.Link eventKey="products">
            <i className="bi bi-box-fill me-2"></i>
            Products
          </Nav.Link>
        </Nav.Item>
      </Nav>
    </Container>
  );
}

export default Navigation;
