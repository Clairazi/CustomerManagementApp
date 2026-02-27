# Customer, Product & Order Management System

A full-stack web application for managing customers, products, and orders with master-detail functionality, built with ASP.NET Core Web API and React. Features JWT-based authentication and role protection.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Authentication](#authentication)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
  - [Database Setup](#database-setup)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [API Documentation](#api-documentation)
- [Project Structure](#project-structure)
- [Usage Guide](#usage-guide)
- [Referential Integrity](#referential-integrity)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

This Customer, Product & Order Management System is a full-featured enterprise application. It provides a comprehensive solution for managing customer records, product inventory, and orders with master-detail functionality (similar to MS Access). Navigate easily between Customer, Product, and Order management using the tab-based navigation. The system enforces referential integrity - customers and products cannot be deleted if they have associated orders.

## ✨ Features

### Customer Management
- **CRUD Operations**: Create, Read, Update, and Delete customer records
- **Advanced Search & Filtering**: Filter customers by first name, last name, email, and phone number
- **Excel Export**: Export customer data to Excel with applied filters
- **Form Validation**: Client-side and server-side validation for data integrity

### Product Management
- **CRUD Operations**: Create, Read, Update, and Delete product records
- **Search & Filtering**: Filter products by name
- **Excel Export**: Export product data to Excel with applied filters
- **Form Validation**: Product name is required, with validation for all fields

### Order Management (Master-Detail)
- **CRUD Operations**: Create, Read, Update, and Delete orders with line items
- **Master-Detail Form**: Order header with dynamic order items (like MS Access)
- **Search & Filtering**: Filter orders by Order ID, Date Range, and Customer
- **Automatic Calculations**: Subtotals and total amounts calculated automatically
- **Status Tracking**: Track order status (Pending, Processing, Completed, Cancelled)

### Referential Integrity
- **Customer Protection**: Customers cannot be deleted if they have orders
- **Product Protection**: Products cannot be deleted if they are used in orders
- **Cascade Delete**: Deleting an order automatically removes its order items

### General Features
- **Navigation**: Tab-based navigation to switch between Customers, Products, and Orders views
- **Responsive Design**: Modern UI built with React and Bootstrap
- **REST API**: Well-documented API endpoints following REST principles
- **Three-Tier Architecture**: Separation of concerns with DAL, BLL, and Presentation layers
- **Data Validation**: Client-side and server-side validation for data integrity

## 🔐 Authentication

### Overview
The application uses JWT (JSON Web Token) based authentication. All API endpoints (except login and register) require a valid JWT token.

### Default User Credentials

| Username | Password  | Role                |
|----------|-----------|---------------------|
| admin    | admin123  | System Administrator|
| user     | user123   | Regular User        |

### Features
- **JWT Token Authentication**: Secure token-based authentication
- **Login/Logout**: Clean login and logout functionality
- **User Registration**: New users can create accounts
- **Protected Routes**: All pages require authentication
- **Auto-logout**: Users are automatically logged out on token expiration or 401 errors
- **Token Persistence**: Authentication state persists across page refreshes (using localStorage)

### Security
- Passwords are hashed using BCrypt with 10 salt rounds
- JWT tokens expire after 24 hours (configurable)
- All protected API endpoints return 401 Unauthorized if no valid token
- Secret key is configurable in `appsettings.json`

### Authentication API Endpoints

#### Login
```
POST /api/auth/login
```
**Request Body:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```
**Response (200 OK):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "username": "admin",
  "fullName": "System Administrator"
}
```
**Response (401 Unauthorized):**
```json
{
  "message": "Invalid username or password"
}
```

#### Register
```
POST /api/auth/register
```
**Request Body:**
```json
{
  "username": "newuser",
  "password": "password123",
  "email": "newuser@example.com",
  "fullName": "New User"
}
```
**Response (201 Created):**
```json
{
  "id": 3,
  "username": "newuser",
  "email": "newuser@example.com",
  "fullName": "New User"
}
```

#### Get Current User (Requires Authentication)
```
GET /api/auth/me
Authorization: Bearer <token>
```
**Response (200 OK):**
```json
{
  "id": 1,
  "username": "admin",
  "email": "admin@example.com",
  "fullName": "System Administrator"
}
```

### Using the JWT Token
All protected endpoints require the token in the Authorization header:
```
Authorization: Bearer <your-jwt-token>
```

## 🛠️ Technology Stack

### Backend
- **Framework**: ASP.NET Core 8.0 Web API
- **ORM**: Entity Framework Core 8.0
- **Database**: SQL Server / SQL Server Express
- **Excel Export**: EPPlus 7.0
- **API Documentation**: Swagger/OpenAPI

### Frontend
- **Framework**: React 18.2
- **Build Tool**: Vite
- **UI Library**: React Bootstrap 2.9 + Bootstrap 5.3
- **HTTP Client**: Axios 1.6

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- **.NET SDK 8.0 or later**: [Download](https://dotnet.microsoft.com/download)
- **Node.js 18.x or later**: [Download](https://nodejs.org/)
- **SQL Server 2019 or later** (or SQL Server Express): [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio Code** or **Visual Studio 2022** (recommended)

### Verify Installation

```bash
# Check .NET version
dotnet --version

# Check Node.js version
node --version

# Check npm version
npm --version
```

## 🏗️ Architecture

The application follows a **Three-Tier Architecture**:

### 1. Data Access Layer (DAL)
- **Entities**: Customer, Product, Order, and OrderItem models with database annotations
- **DbContext**: Entity Framework Core context for database operations
- **Repository Pattern**: Abstraction for data access operations
  - `ICustomerRepository` / `CustomerRepository`
  - `IProductRepository` / `ProductRepository`
  - `IOrderRepository` / `OrderRepository`
- **Relationships**:
  - Order → Customer (Many-to-One, Restrict Delete)
  - Order → OrderItems (One-to-Many, Cascade Delete)
  - OrderItem → Product (Many-to-One, Restrict Delete)

### 2. Business Logic Layer (BLL)
- **Services**: Business logic and validation
  - `ICustomerService` / `CustomerService`
  - `IProductService` / `ProductService`
  - `IOrderService` / `OrderService`
- **DTOs**: Data Transfer Objects for API communication
  - Customer: `CustomerDto`, `CreateCustomerDto`, `UpdateCustomerDto`
  - Product: `ProductDto`, `CreateProductDto`, `UpdateProductDto`
  - Order: `OrderDto`, `OrderItemDto`, `CreateOrderDto`, `CreateOrderItemDto`, `UpdateOrderDto`, `UpdateOrderItemDto`
- **Integrity Checks**: Services validate referential integrity before delete operations

### 3. Presentation Layer (API)
- **Controllers**: REST API endpoints
  - `CustomersController`
  - `ProductsController`
  - `OrdersController`
- **Middleware**: CORS, Error Handling, Swagger

## 🚀 Getting Started

### Database Setup

1. **Install SQL Server**
   - Download and install SQL Server or SQL Server Express
   - Note your server name (e.g., `localhost` or `.\SQLEXPRESS`)

2. **Update Connection String**
   
   Open `CustomerManagementAPI/appsettings.json` and update the connection string:
   
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=CustomerManagementDB;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```
   
   **Common connection strings:**
   - Local SQL Server: `Server=localhost;Database=CustomerManagementDB;Trusted_Connection=True;TrustServerCertificate=True;`
   - SQL Server Express: `Server=.\SQLEXPRESS;Database=CustomerManagementDB;Trusted_Connection=True;TrustServerCertificate=True;`
   - SQL Server with credentials: `Server=localhost;Database=CustomerManagementDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;`

3. **Create Database and Run Migrations**
   
   ```bash
   cd CustomerManagementAPI
   
   # Install EF Core tools (if not already installed)
   dotnet tool install --global dotnet-ef
   
   # Restore packages
   dotnet restore
   
   # Create database and apply migrations
   dotnet ef database update
   ```
   
   This will:
   - Create the `CustomerManagementDB` database
   - Create the `Customers`, `Products`, `Orders`, and `OrderItems` tables
   - Seed initial sample data (2 customers, 2 products, 2 sample orders)

### Backend Setup

1. **Navigate to the API directory**
   ```bash
   cd CustomerManagementAPI
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the API**
   ```bash
   dotnet run
   ```
   
   The API will start on:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`
   - Swagger UI: `http://localhost:5000/swagger`

### Frontend Setup

1. **Open a new terminal and navigate to the ClientApp directory**
   ```bash
   cd ClientApp
   ```

2. **Install npm packages**
   ```bash
   npm install
   ```

3. **Start the development server**
   ```bash
   npm run dev
   ```
   
   The React app will start on: `http://localhost:3000`

4. **Open your browser**
   
   Navigate to `http://localhost:3000` to access the application.

## 📚 API Documentation

### Base URL
```
http://localhost:5000/api
```

### Endpoints

#### 1. Get All Customers
```
GET /api/customers
```

**Query Parameters** (all optional):
- `firstName` (string): Filter by first name
- `lastName` (string): Filter by last name
- `email` (string): Filter by email
- `phoneNumber` (string): Filter by phone number

**Example:**
```
GET /api/customers?firstName=John&email=example.com
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "firstName": "John",
    "lastName": "Doe",
    "email": "john.doe@example.com",
    "phoneNumber": "555-1234",
    "createdAt": "2026-02-24T10:30:00Z",
    "updatedAt": null
  }
]
```

#### 2. Get Customer by ID
```
GET /api/customers/{id}
```

**Response:** `200 OK` or `404 Not Found`

#### 3. Create Customer
```
POST /api/customers
```

**Request Body:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john.doe@example.com",
  "phoneNumber": "555-1234"
}
```

**Required Fields:**
- `firstName` (string, max 100 characters)
- `lastName` (string, max 100 characters)

**Optional Fields:**
- `email` (string, must be valid email, max 200 characters)
- `phoneNumber` (string, max 20 characters)

**Response:** `201 Created`

#### 4. Update Customer
```
PUT /api/customers/{id}
```

**Request Body:** Same as Create Customer

**Response:** `200 OK` or `404 Not Found`

#### 5. Delete Customer
```
DELETE /api/customers/{id}
```

**Response:** `204 No Content` or `404 Not Found`

#### 6. Export Customers to Excel
```
GET /api/customers/export
```

**Query Parameters:** Same as Get All Customers

**Response:** Excel file download

---

### Products API Endpoints

#### 1. Get All Products
```
GET /api/products
```

**Query Parameters** (optional):
- `name` (string): Filter by product name

**Example:**
```
GET /api/products?name=Laptop
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "name": "Laptop Computer",
    "description": "High-performance laptop with 16GB RAM",
    "price": 999.99,
    "sku": "LAPTOP-001",
    "createdAt": "2026-02-24T10:30:00Z",
    "updatedAt": null
  }
]
```

#### 2. Get Product by ID
```
GET /api/products/{id}
```

**Response:** `200 OK` or `404 Not Found`

#### 3. Create Product
```
POST /api/products
```

**Request Body:**
```json
{
  "name": "New Product",
  "description": "Product description",
  "price": 49.99,
  "sku": "PROD-001"
}
```

**Required Fields:**
- `name` (string, max 200 characters)

**Optional Fields:**
- `description` (string, max 1000 characters)
- `price` (decimal, must be >= 0)
- `sku` (string, max 50 characters)

**Response:** `201 Created`

#### 4. Update Product
```
PUT /api/products/{id}
```

**Request Body:** Same as Create Product

**Response:** `200 OK` or `404 Not Found`

#### 5. Delete Product
```
DELETE /api/products/{id}
```

**Response:** `204 No Content` or `404 Not Found`

#### 6. Export Products to Excel
```
GET /api/products/export
```

**Query Parameters:** Same as Get All Products

**Response:** Excel file download

---

### Orders API Endpoints

#### 1. Get All Orders
```
GET /api/orders
```

**Query Parameters** (all optional):
- `orderId` (integer): Filter by order ID
- `dateFrom` (date): Filter orders from this date
- `dateTo` (date): Filter orders up to this date
- `customerId` (integer): Filter by customer ID

**Example:**
```
GET /api/orders?dateFrom=2026-02-01&dateTo=2026-02-28&customerId=1
```

**Response:** `200 OK`
```json
[
  {
    "id": 1,
    "customerId": 1,
    "customerName": "John Doe",
    "orderDate": "2026-02-01T10:00:00Z",
    "totalAmount": 1029.98,
    "status": "Completed",
    "createdAt": "2026-02-01T10:00:00Z",
    "updatedAt": null,
    "orderItems": [
      {
        "id": 1,
        "productId": 1,
        "productName": "Laptop Computer",
        "quantity": 1,
        "unitPrice": 999.99,
        "subtotal": 999.99
      },
      {
        "id": 2,
        "productId": 2,
        "productName": "Wireless Mouse",
        "quantity": 1,
        "unitPrice": 29.99,
        "subtotal": 29.99
      }
    ]
  }
]
```

#### 2. Get Order by ID
```
GET /api/orders/{id}
```

**Response:** `200 OK` or `404 Not Found`

#### 3. Create Order (Master-Detail)
```
POST /api/orders
```

**Request Body:**
```json
{
  "customerId": 1,
  "orderDate": "2026-02-24",
  "status": "Pending",
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2,
      "unitPrice": 999.99
    },
    {
      "productId": 2,
      "quantity": 3,
      "unitPrice": 29.99
    }
  ]
}
```

**Required Fields:**
- `customerId` (integer): Must reference an existing customer
- `orderItems` (array): At least one item required
  - `productId` (integer): Must reference an existing product
  - `quantity` (integer): Must be at least 1
  - `unitPrice` (decimal): Must be greater than 0

**Optional Fields:**
- `orderDate` (date): Defaults to current date
- `status` (string): Defaults to "Pending"

**Response:** `201 Created` with calculated totals

#### 4. Update Order
```
PUT /api/orders/{id}
```

**Request Body:** Same structure as Create Order

**Response:** `200 OK` or `404 Not Found`

#### 5. Delete Order
```
DELETE /api/orders/{id}
```

**Response:** `204 No Content` or `404 Not Found`

**Note:** Deleting an order automatically deletes all associated order items (cascade delete).

---

### Delete Operations with Referential Integrity

#### Delete Customer
```
DELETE /api/customers/{id}
```

**Response:**
- `204 No Content`: Customer deleted successfully
- `400 Bad Request`: Customer has orders (cannot delete)
- `404 Not Found`: Customer not found

#### Delete Product
```
DELETE /api/products/{id}
```

**Response:**
- `204 No Content`: Product deleted successfully
- `400 Bad Request`: Product is used in orders (cannot delete)
- `404 Not Found`: Product not found

---

### Testing with Swagger

Access the Swagger UI at `http://localhost:5000/swagger` to test all API endpoints interactively.

## 📁 Project Structure

```
customer_management_app/
├── CustomerManagementAPI/           # Backend ASP.NET Core Web API
│   ├── BLL/                        # Business Logic Layer
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   │   ├── CustomerDto.cs
│   │   │   ├── ProductDto.cs
│   │   │   ├── OrderDto.cs         # Order DTOs (master-detail)
│   │   │   └── AuthDto.cs          # Login, Register, User DTOs
│   │   └── Services/               # Business Services
│   │       ├── ICustomerService.cs
│   │       ├── CustomerService.cs  # Includes integrity check
│   │       ├── IProductService.cs
│   │       ├── ProductService.cs   # Includes integrity check
│   │       ├── IOrderService.cs
│   │       ├── OrderService.cs     # Master-detail operations
│   │       ├── IAuthService.cs     # Authentication interface
│   │       └── AuthService.cs      # JWT token generation
│   ├── Controllers/                # API Controllers
│   │   ├── CustomersController.cs  # [Authorize] protected
│   │   ├── ProductsController.cs   # [Authorize] protected
│   │   ├── OrdersController.cs     # [Authorize] protected
│   │   └── AuthController.cs       # Login, Register, Me endpoints
│   ├── DAL/                        # Data Access Layer
│   │   ├── Entities/               # Database Entities
│   │   │   ├── Customer.cs
│   │   │   ├── Product.cs
│   │   │   ├── Order.cs            # Master entity
│   │   │   ├── OrderItem.cs        # Detail entity
│   │   │   └── User.cs             # User entity for auth
│   │   ├── Migrations/             # EF Core Migrations
│   │   ├── Repositories/           # Repository Pattern
│   │   │   ├── ICustomerRepository.cs
│   │   │   ├── CustomerRepository.cs
│   │   │   ├── IProductRepository.cs
│   │   │   ├── ProductRepository.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   ├── OrderRepository.cs
│   │   │   ├── IUserRepository.cs  # User repository interface
│   │   │   └── UserRepository.cs   # User data access
│   │   └── ApplicationDbContext.cs # DbSets + relationships + Users
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── appsettings.json            # Includes JwtSettings
│   ├── appsettings.Development.json
│   ├── CustomerManagementAPI.csproj
│   └── Program.cs                  # JWT auth configuration
├── ClientApp/                      # Frontend React Application
│   ├── public/
│   ├── src/
│   │   ├── components/             # React Components
│   │   │   ├── CustomerList.jsx    # Handles integrity errors
│   │   │   ├── CustomerForm.jsx
│   │   │   ├── ProductList.jsx     # Handles integrity errors
│   │   │   ├── ProductForm.jsx
│   │   │   ├── OrderList.jsx       # Order listing with filters
│   │   │   ├── OrderForm.jsx       # Master-detail form
│   │   │   ├── Navigation.jsx
│   │   │   ├── Login.jsx           # Login form component
│   │   │   └── Register.jsx        # Registration form
│   │   ├── context/                # React Context
│   │   │   └── AuthContext.jsx     # Authentication state management
│   │   ├── services/               # API Services
│   │   │   ├── customerService.js  # Includes auth headers
│   │   │   ├── productService.js   # Includes auth headers
│   │   │   ├── orderService.js     # Includes auth headers
│   │   │   └── authService.js      # Login, register, token mgmt
│   │   ├── App.jsx                 # Auth-protected layout
│   │   ├── App.css
│   │   ├── index.css
│   │   └── main.jsx
│   ├── index.html
│   ├── package.json
│   └── vite.config.js
├── .gitignore
└── README.md
```

## 📖 Usage Guide

### Adding a New Customer

1. Click the **"Add New Customer"** button
2. Fill in the required fields (First Name, Last Name)
3. Optionally add Email and Phone Number
4. Click **"Create Customer"**

### Editing a Customer

1. Find the customer in the list
2. Click the **"Edit"** button
3. Modify the fields as needed
4. Click **"Update Customer"**

### Deleting a Customer

1. Find the customer in the list
2. Click the **"Delete"** button
3. Confirm the deletion in the modal dialog

### Filtering/Searching Customers

1. Enter search criteria in any of the filter fields:
   - First Name
   - Last Name
   - Email
   - Phone Number
2. Click **"Search"** to apply filters
3. Click **"Clear Filters"** to reset and show all customers

### Exporting to Excel

1. Optionally apply filters to export specific customers
2. Click **"Export to Excel"**
3. The Excel file will download automatically

---

### Navigating Between Modules

1. Click on the **"Customers"**, **"Products"**, or **"Orders"** tab in the navigation bar
2. The view will switch to the selected module
3. Each module maintains its own state

### Adding a New Product

1. Navigate to the Products tab
2. Click the **"Add New Product"** button
3. Fill in the required field (Product Name)
4. Optionally add Description, Price, and SKU
5. Click **"Create Product"**

### Editing a Product

1. Find the product in the list
2. Click the **"Edit"** button
3. Modify the fields as needed
4. Click **"Update Product"**

### Deleting a Product

1. Find the product in the list
2. Click the **"Delete"** button
3. Confirm the deletion in the modal dialog

### Filtering/Searching Products

1. Enter a product name in the filter field
2. Click **"Search"** to apply the filter
3. Click **"Clear Filters"** to reset and show all products

### Exporting Products to Excel

1. Optionally apply name filter to export specific products
2. Click **"Export to Excel"**
3. The Excel file will download automatically

---

### Creating a New Order (Master-Detail)

1. Navigate to the **Orders** tab
2. Click **"Add New Order"**
3. **Order Header (Master)**:
   - Select a customer from the dropdown
   - Choose the order date
   - Set the status (Pending, Processing, Completed, Cancelled)
4. **Order Items (Detail)**:
   - Select a product from the dropdown (unit price auto-fills)
   - Enter the quantity
   - Adjust unit price if needed
   - Subtotal calculates automatically
   - Click **"Add Item"** to add more products
   - Click the trash icon to remove an item
5. Review the **Total Amount** (calculated automatically)
6. Click **"Create Order"**

### Editing an Order

1. Find the order in the list
2. Click the **"Edit"** button
3. Modify order details or items as needed
4. Click **"Update Order"**

### Deleting an Order

1. Find the order in the list
2. Click the **"Delete"** button
3. Confirm the deletion (this will also delete all order items)

### Filtering/Searching Orders

1. Enter search criteria:
   - Order ID (exact match)
   - Date From (orders from this date)
   - Date To (orders up to this date)
   - Customer (select from dropdown)
2. Click **"Search"** to apply filters
3. Click **"Clear Filters"** to reset

## 🔒 Referential Integrity

The system enforces referential integrity to maintain data consistency:

### Customer Protection
- **Rule**: Customers cannot be deleted if they have orders
- **Behavior**: When attempting to delete a customer with orders, you'll see an error message
- **Resolution**: Delete all orders for the customer first, then delete the customer

### Product Protection
- **Rule**: Products cannot be deleted if they are used in any order
- **Behavior**: When attempting to delete a product that's in an order, you'll see an error message
- **Resolution**: Delete all orders containing the product first, then delete the product

### Order Cascade Delete
- **Rule**: When an order is deleted, all its order items are automatically deleted
- **Behavior**: Deleting an order removes it completely along with all line items

## 🔧 Troubleshooting

### Database Connection Issues

**Problem**: Cannot connect to SQL Server

**Solutions**:
- Verify SQL Server is running
- Check your connection string in `appsettings.json`
- Ensure SQL Server allows TCP/IP connections
- For SQL Server Express, use `.\SQLEXPRESS` as the server name

### Migration Errors

**Problem**: `dotnet ef database update` fails

**Solutions**:
```bash
# Install/Update EF Core tools
dotnet tool install --global dotnet-ef --version 8.0.0

# Clean and rebuild
dotnet clean
dotnet build

# Try migration again
dotnet ef database update
```

### CORS Errors

**Problem**: API calls fail with CORS errors

**Solutions**:
- Ensure the backend is running on `http://localhost:5000`
- Ensure the frontend is running on `http://localhost:3000`
- Check CORS configuration in `Program.cs`

### Port Already in Use

**Problem**: Port 3000 or 5000 is already in use

**Solutions**:
- Change the port in `vite.config.js` (frontend) or `launchSettings.json` (backend)
- Kill the process using the port
- On Windows: `netstat -ano | findstr :5000` then `taskkill /PID <PID> /F`
- On Linux/Mac: `lsof -ti:5000 | xargs kill -9`

### Excel Export Issues

**Problem**: Excel export fails or doesn't download

**Solutions**:
- Check browser console for errors
- Ensure EPPlus package is installed: `dotnet add package EPPlus`
- Check pop-up blocker settings in your browser

## 🔐 Important Notes

- **This is a development setup**: Not production-ready without additional security measures
- **Authentication**: JWT-based authentication is implemented ✅
- **Default Users**: The system includes two default users (admin/admin123, user/user123)
- **localhost**: The backend runs on localhost of the development machine. To access remotely or deploy, you'll need to deploy the application on your own server/hosting environment.

## 🎯 Future Enhancements (Planned)

- ~~Authentication and Authorization~~ ✅ **Implemented**
- ~~Orders Management (linking Customers and Products)~~ ✅ **Implemented**
- Role-based Access Control
- Audit Logging
- Advanced Reporting
- Order Excel Export
- Docker Support

## 📄 License

This project is created for demonstration purposes.

## 👨‍💻 Support

For issues or questions, please check the troubleshooting section above or consult the code comments for detailed explanations.
