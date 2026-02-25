using CustomerManagementAPI.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CustomerManagementAPI.DAL
{
    /// <summary>
    /// Database context for Customer Management application
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

            // Seed initial Customer data
            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    PhoneNumber = "555-1234",
                    CreatedAt = DateTime.UtcNow
                },
                new Customer
                {
                    Id = 2,
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    PhoneNumber = "555-5678",
                    CreatedAt = DateTime.UtcNow
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
                    CreatedAt = DateTime.UtcNow
                },
                new Product
                {
                    Id = 2,
                    Name = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with Bluetooth",
                    Price = 29.99m,
                    SKU = "MOUSE-001",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}