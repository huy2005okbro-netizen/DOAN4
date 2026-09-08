using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // ===== DbSets =====
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<ShippingMethod> ShippingMethods => Set<ShippingMethod>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Shipment> Shipments => Set<Shipment>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Promotion> Promotions => Set<Promotion>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== Role =====
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            // ===== User =====
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique();

            // User -> Role (Many-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Customer =====
            // Customer -> User (One-to-One)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.User)
                .WithOne(u => u.Customer)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Employee =====
            // Employee -> User (One-to-One)
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithOne(u => u.Employee)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Service =====
            modelBuilder.Entity<Service>()
                .HasOne(s => s.ServiceCategory)
                .WithMany(sc => sc.Services)
                .HasForeignKey(s => s.ServiceCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Appointment =====
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Customer)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Service)
                .WithMany(s => s.Appointments)
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Appointments)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Room)
                .WithMany(r => r.Appointments)
                .HasForeignKey(a => a.RoomId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== Product =====
            modelBuilder.Entity<Product>()
                .HasOne(p => p.ProductCategory)
                .WithMany(pc => pc.Products)
                .HasForeignKey(p => p.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Cart =====
            // Cart -> Customer (One-to-One)
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Customer)
                .WithOne(cu => cu.Cart)
                .HasForeignKey<Cart>(c => c.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== CartItem =====
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Order =====
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.OrderCode)
                .IsUnique();

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== OrderItem =====
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Shipment =====
            // Shipment -> Order (One-to-One)
            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.Order)
                .WithOne(o => o.Shipment)
                .HasForeignKey<Shipment>(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Shipment>()
                .HasOne(s => s.ShippingMethod)
                .WithMany(sm => sm.Shipments)
                .HasForeignKey(s => s.ShippingMethodId)
                .OnDelete(DeleteBehavior.SetNull);

            // ===== Payment =====
            // Payment -> Order (One-to-One)
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Invoice =====
            // Invoice -> Order (One-to-One)
            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceCode)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Order)
                .WithOne(o => o.Invoice)
                .HasForeignKey<Invoice>(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Voucher =====
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();

            modelBuilder.Entity<Voucher>()
                .HasOne(v => v.Promotion)
                .WithMany(p => p.Vouchers)
                .HasForeignKey(v => v.PromotionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ===== Review =====
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Service)
                .WithMany(s => s.Reviews)
                .HasForeignKey(r => r.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== InventoryTransaction =====
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(it => it.Product)
                .WithMany(p => p.InventoryTransactions)
                .HasForeignKey(it => it.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // ===== Seed Data =====
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Roles
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "ADMIN" },
                new Role { Id = 2, Name = "EMPLOYEE" },
                new Role { Id = 3, Name = "CUSTOMER" }
            );

            // Seed Admin user - password hash cho "Admin@123" được tạo sẵn (cố định)
            // Hash này được tạo bởi BCrypt.HashPassword("Admin@123") với workfactor=11
            const string adminPasswordHash = "$2a$11$xMOY5sDtKfqRqUYeBTDuKOvMpiQ5R8RR.R/iSFpyWsGi1gkrRjlHi";

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Administrator",
                    Email = "admin@massageshop.com",
                    Phone = "0900000000",
                    PasswordHash = adminPasswordHash,
                    RoleId = 1,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
