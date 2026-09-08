using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Data
{
    /// <summary>
    /// Seed dữ liệu mẫu khi khởi động (chỉ insert nếu chưa có).
    /// Không dùng HasData() để tránh vấn đề BCrypt hash thay đổi giữa các migration.
    /// </summary>
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext db)
        {
            // Migrate database
            await db.Database.MigrateAsync();

            // ===== Seed Roles =====
            if (!await db.Roles.AnyAsync())
            {
                db.Roles.AddRange(
                    new Role { Name = "ADMIN" },
                    new Role { Name = "EMPLOYEE" },
                    new Role { Name = "CUSTOMER" }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed Admin User =====
            if (!await db.Users.AnyAsync(u => u.Email == "admin@massageshop.com"))
            {
                var adminRole = await db.Roles.FirstAsync(r => r.Name == "ADMIN");

                var adminUser = new User
                {
                    FullName = "Administrator",
                    Email = "admin@massageshop.com",
                    Phone = "0900000000",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    RoleId = adminRole.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                db.Users.Add(adminUser);
                await db.SaveChangesAsync();
            }

            // ===== Seed ShippingMethods =====
            if (!await db.ShippingMethods.AnyAsync())
            {
                db.ShippingMethods.AddRange(
                    new ShippingMethod { Name = "Giao hàng tiêu chuẩn", Description = "3-5 ngày làm việc", Fee = 30000, IsActive = true },
                    new ShippingMethod { Name = "Giao hàng nhanh", Description = "1-2 ngày làm việc", Fee = 50000, IsActive = true },
                    new ShippingMethod { Name = "Giao hàng hỏa tốc", Description = "Trong ngày", Fee = 80000, IsActive = true }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed ServiceCategories =====
            if (!await db.ServiceCategories.AnyAsync())
            {
                db.ServiceCategories.AddRange(
                    new ServiceCategory { Name = "Massage thư giãn", Description = "Các dịch vụ massage thư giãn toàn thân", IsActive = true },
                    new ServiceCategory { Name = "Massage trị liệu", Description = "Massage điều trị đau nhức, phục hồi", IsActive = true },
                    new ServiceCategory { Name = "Chăm sóc da mặt", Description = "Các liệu trình chăm sóc da mặt", IsActive = true }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed Services =====
            if (!await db.Services.AnyAsync())
            {
                var cat1 = await db.ServiceCategories.FirstAsync(c => c.Name == "Massage thư giãn");
                var cat2 = await db.ServiceCategories.FirstAsync(c => c.Name == "Massage trị liệu");

                db.Services.AddRange(
                    new Service
                    {
                        ServiceCategoryId = cat1.Id,
                        Name = "Massage toàn thân 60 phút",
                        Description = "Thư giãn toàn thân với các kỹ thuật massage Thái",
                        Duration = 60,
                        Price = 350000,
                        IsActive = true
                    },
                    new Service
                    {
                        ServiceCategoryId = cat1.Id,
                        Name = "Massage toàn thân 90 phút",
                        Description = "Thư giãn toàn thân chuyên sâu",
                        Duration = 90,
                        Price = 500000,
                        IsActive = true
                    },
                    new Service
                    {
                        ServiceCategoryId = cat2.Id,
                        Name = "Massage trị liệu cột sống",
                        Description = "Điều trị đau lưng, cột sống",
                        Duration = 75,
                        Price = 450000,
                        IsActive = true
                    }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed Rooms =====
            if (!await db.Rooms.AnyAsync())
            {
                db.Rooms.AddRange(
                    new Room { RoomNumber = "P01", RoomType = "VIP", Capacity = 2, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "P02", RoomType = "Standard", Capacity = 1, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "P03", RoomType = "Standard", Capacity = 1, Status = RoomStatus.AVAILABLE },
                    new Room { RoomNumber = "P04", RoomType = "VIP", Capacity = 2, Status = RoomStatus.AVAILABLE }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed ProductCategories =====
            if (!await db.ProductCategories.AnyAsync())
            {
                db.ProductCategories.AddRange(
                    new ProductCategory { Name = "Dầu massage", Description = "Các loại dầu dùng trong massage", IsActive = true },
                    new ProductCategory { Name = "Kem dưỡng da", Description = "Kem dưỡng ẩm và chăm sóc da", IsActive = true },
                    new ProductCategory { Name = "Thiết bị massage", Description = "Các thiết bị hỗ trợ massage tại nhà", IsActive = true }
                );
                await db.SaveChangesAsync();
            }

            // ===== Seed Products =====
            if (!await db.Products.AnyAsync())
            {
                var oilCat = await db.ProductCategories.FirstAsync(c => c.Name == "Dầu massage");
                var creamCat = await db.ProductCategories.FirstAsync(c => c.Name == "Kem dưỡng da");

                db.Products.AddRange(
                    new Product
                    {
                        ProductCategoryId = oilCat.Id,
                        Name = "Dầu massage oải hương 100ml",
                        Description = "Dầu massage tinh chất oải hương thư giãn",
                        Price = 150000,
                        CostPrice = 70000,
                        StockQuantity = 50,
                        Brand = "Aromatherapy",
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCategoryId = oilCat.Id,
                        Name = "Dầu massage bạc hà 100ml",
                        Description = "Dầu massage bạc hà mát lạnh giảm đau",
                        Price = 130000,
                        CostPrice = 60000,
                        StockQuantity = 30,
                        Brand = "Aromatherapy",
                        IsActive = true
                    },
                    new Product
                    {
                        ProductCategoryId = creamCat.Id,
                        Name = "Kem dưỡng ẩm Body 200ml",
                        Description = "Kem dưỡng ẩm toàn thân sau massage",
                        Price = 200000,
                        CostPrice = 100000,
                        StockQuantity = 40,
                        Brand = "SkinCare Pro",
                        IsActive = true
                    }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
