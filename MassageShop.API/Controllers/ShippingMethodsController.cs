using MassageShop.API.Data;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/shipping-methods")]
    public class ShippingMethodsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ShippingMethodsController(AppDbContext db) => _db = db;

        /// <summary>Lấy danh sách phương thức vận chuyển - Public</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _db.ShippingMethods
                .Where(s => s.IsActive)
                .ToListAsync();
            return Ok(result);
        }

        /// <summary>Lấy tất cả (bao gồm inactive) - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllAdmin() =>
            Ok(await _db.ShippingMethods.ToListAsync());

        /// <summary>Lấy theo ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _db.ShippingMethods.FindAsync(id);
            return result == null ? NotFound(new { message = "Không tìm thấy phương thức vận chuyển" }) : Ok(result);
        }

        /// <summary>Tạo phương thức vận chuyển - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShippingMethod method)
        {
            _db.ShippingMethods.Add(method);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = method.Id }, method);
        }

        /// <summary>Cập nhật - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShippingMethod updated)
        {
            var method = await _db.ShippingMethods.FindAsync(id);
            if (method == null) return NotFound();

            method.Name = updated.Name;
            method.Description = updated.Description;
            method.Fee = updated.Fee;
            method.IsActive = updated.IsActive;

            await _db.SaveChangesAsync();
            return Ok(method);
        }

        /// <summary>Xóa - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var method = await _db.ShippingMethods.FindAsync(id);
            if (method == null) return NotFound();
            method.IsActive = false;
            await _db.SaveChangesAsync();
            return Ok(new { message = "Đã ẩn phương thức vận chuyển" });
        }
    }
}
