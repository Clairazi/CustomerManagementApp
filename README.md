# Customer, Product & Order Management System

A full-stack web application for managing customers, products, and orders with master-detail functionality, built with ASP.NET Core Web API and React. Features JWT-based authentication and role protection.

## 🎯 Overview

This Customer, Product & Order Management System is a full-featured enterprise application. It provides a comprehensive solution for managing customer records, product inventory, and orders with master-detail functionality. Navigate easily between Customer, Product, and Order management using the tab-based navigation. The system enforces referential integrity - customers and products cannot be deleted if they have associated orders.

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
- **Master-Detail Form**: Order header with dynamic order items
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
