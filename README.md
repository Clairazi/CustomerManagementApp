# Customer, Product & Order Management System

A full-stack web application for managing customers, products, and orders with master-detail functionality, built with ASP.NET Core Web API and React. Features JWT-based authentication and role protection.

## Authentication

### Overview
The application uses JWT (JSON Web Token) based authentication. All API endpoints (except login and register) require a valid JWT token.

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET SDK 8.0 or later**: [Download](https://dotnet.microsoft.com/download)
- **Node.js 18.x or later**: [Download](https://nodejs.org/)
- **SQL Server 2019 or later** (or SQL Server Express): [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- **Visual Studio Code** or **Visual Studio 2022**

### Verify Installation

```bash
# Check .NET version
dotnet --version

# Check Node.js version
node --version

# Check npm version
npm --version
```

## Architecture

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

## Getting Started

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
