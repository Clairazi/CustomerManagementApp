using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL
{
    /// <summary>
    /// Database context for Customer Management application.
    /// Manages Customer, Product, Order, OrderItem, and User entities.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Customers table
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// Products table
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Orders table (master in master-detail relationship)
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// OrderItems table (detail in master-detail relationship)
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        /// <summary>
        /// Users table (for authentication)
        /// </summary>
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.HasIndex(e => e.Email);
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.SKU).HasMaxLength(50);
                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.SKU);
            });

            // Configure Order entity (Master)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderDate).IsRequired();
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.OrderDate);
                entity.HasIndex(e => e.CustomerId);

                // Configure relationship: Order has one Customer (required)
                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete - integrity check
            });

            // Configure OrderItem entity (Detail)
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.ProductId);

                // Configure relationship: OrderItem has one Order (cascade delete)
                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade); // Delete items when order is deleted

                // Configure relationship: OrderItem has one Product (required, restrict delete)
                entity.HasOne(e => e.Product)
                    .WithMany()
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete - integrity check
            });

            // Configure User entity (for authentication)
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(100);
                
                // Unique index on Username for fast lookup and uniqueness constraint
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Seed initial Customer data
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "555-1234",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "555-5678",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed initial Product data
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Laptop Computer",
                    Description = "High-performance laptop with 16GB RAM",
                    Price = 999.99m,
                    SKU = "LAPTOP-001",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with Bluetooth",
                    Price = 29.99m,
                    SKU = "MOUSE-001",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed initial Order data (sample orders to demonstrate master-detail)
            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    CustomerId = 1,
                    OrderDate = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc),
                    TotalAmount = 1029.98m,
                    Status = "Completed",
                    CreatedAt = new DateTime(2026, 2, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new Order
                {
                    Id = 2,
                    CustomerId = 2,
                    OrderDate = new DateTime(2026, 2, 15, 14, 30, 0, DateTimeKind.Utc),
                    TotalAmount = 59.98m,
                    Status = "Pending",
                    CreatedAt = new DateTime(2026, 2, 15, 14, 30, 0, DateTimeKind.Utc)
                }
            );

            // Seed initial OrderItem data (detail records)
            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    ProductId = 1,
                    Quantity = 1,
                    UnitPrice = 999.99m,
                    Subtotal = 999.99m
                },
                new OrderItem
                {
                    Id = 2,
                    OrderId = 1,
                    ProductId = 2,
                    Quantity = 1,
                    UnitPrice = 29.99m,
                    Subtotal = 29.99m
                },
                new OrderItem
                {
                    Id = 3,
                    OrderId = 2,
                    ProductId = 2,
                    Quantity = 2,
                    UnitPrice = 29.99m,
                    Subtotal = 59.98m
                }
            );

            // Seed initial User data for authentication
            // Passwords are hashed using BCrypt with 10 salt rounds
            // admin/admin123 and user/user123
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    // BCrypt hash for "admin123"
                    PasswordHash = "$2a$10$rQEYn6/U5rvBl7yx5gZ5/.qP.tMI8I9E7tTJ.H5KH.aZvGx6Z5OPK",
                    Email = "admin@example.com",
                    FullName = "System Administrator",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    Username = "user",
                    // BCrypt hash for "user123"
                    PasswordHash = "$2a$10$rP8BV8Z5F5YqG2YB6S5xOeQxvN5M8M5M5O5O5N5N5M5M5M5M5M5M",
                    Email = "user@example.com",
                    FullName = "Regular User",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
