using MassageShop.API.Data;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "ADMIN")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db) => _db = db;

        /// <summary>Tổng quan doanh thu theo khoảng ngày</summary>
        [HttpGet("revenue")]
        public async Task<IActionResult> Revenue(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var orders = await _db.Orders
                .Where(o => o.CreatedAt >= from && o.CreatedAt <= to.AddDays(1)
                         && o.Status == OrderStatus.COMPLETED)
                .ToListAsync();

            return Ok(new
            {
                From = from,
                To = to,
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                OnlineRevenue = orders.Where(o => o.OrderType == OrderType.ONLINE).Sum(o => o.TotalAmount),
                InStoreRevenue = orders.Where(o => o.OrderType == OrderType.IN_STORE).Sum(o => o.TotalAmount)
            });
        }

        /// <summary>Thống kê đặt lịch theo khoảng ngày</summary>
        [HttpGet("appointments")]
        public async Task<IActionResult> Appointments(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var appointments = await _db.Appointments
                .Where(a => a.AppointmentDate >= from && a.AppointmentDate <= to.AddDays(1))
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            return Ok(new
            {
                From = from,
                To = to,
                ByStatus = appointments
            });
        }

        /// <summary>Top sản phẩm bán chạy</summary>
        [HttpGet("top-products")]
        public async Task<IActionResult> TopProducts([FromQuery] int top = 10)
        {
            var result = await _db.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .Where(oi => oi.Order.Status == OrderStatus.COMPLETED)
                .GroupBy(oi => new { oi.ProductId, oi.Product.Name })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.Name,
                    TotalQuantity = g.Sum(oi => oi.Quantity),
                    TotalRevenue = g.Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(top)
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>Top dịch vụ được đặt nhiều nhất</summary>
        [HttpGet("top-services")]
        public async Task<IActionResult> TopServices([FromQuery] int top = 10)
        {
            var result = await _db.Appointments
                .Include(a => a.Service)
                .Where(a => a.Status == AppointmentStatus.COMPLETED)
                .GroupBy(a => new { a.ServiceId, a.Service.Name })
                .Select(g => new
                {
                    ServiceId = g.Key.ServiceId,
                    ServiceName = g.Key.Name,
                    TotalBookings = g.Count()
                })
                .OrderByDescending(x => x.TotalBookings)
                .Take(top)
                .ToListAsync();

            return Ok(result);
        }

        /// <summary>Tổng quan hệ thống</summary>
        [HttpGet("overview")]
        public async Task<IActionResult> Overview()
        {
            var totalCustomers = await _db.Customers.CountAsync();
            var totalEmployees = await _db.Employees.CountAsync(e => e.IsActive);
            var totalProducts = await _db.Products.CountAsync(p => p.IsActive);
            var totalServices = await _db.Services.CountAsync(s => s.IsActive);

            var today = DateTime.UtcNow.Date;
            var todayOrders = await _db.Orders
                .Where(o => o.CreatedAt >= today)
                .CountAsync();
            var todayRevenue = await _db.Orders
                .Where(o => o.CreatedAt >= today && o.Status == OrderStatus.COMPLETED)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            var pendingAppointments = await _db.Appointments
                .CountAsync(a => a.Status == AppointmentStatus.PENDING);
            var pendingOrders = await _db.Orders
                .CountAsync(o => o.Status == OrderStatus.PENDING);

            return Ok(new
            {
                TotalCustomers = totalCustomers,
                TotalEmployees = totalEmployees,
                TotalProducts = totalProducts,
                TotalServices = totalServices,
                TodayOrders = todayOrders,
                TodayRevenue = todayRevenue,
                PendingAppointments = pendingAppointments,
                PendingOrders = pendingOrders
            });
        }

        /// <summary>Tồn kho sản phẩm</summary>
        [HttpGet("inventory")]
        public async Task<IActionResult> InventoryReport([FromQuery] bool lowStockOnly = false)
        {
            var query = _db.Products
                .Include(p => p.ProductCategory)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (lowStockOnly)
                query = query.Where(p => p.StockQuantity <= 10);

            var result = await query
                .OrderBy(p => p.StockQuantity)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    CategoryName = p.ProductCategory.Name,
                    p.StockQuantity,
                    p.Price,
                    p.Brand
                })
                .ToListAsync();

            return Ok(result);
        }
    }
}
