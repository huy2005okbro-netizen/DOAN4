using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(ICustomerService service) => _service = service;

        /// <summary>Lấy danh sách khách hàng - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll() =>
            Ok(await _service.GetAllAsync());

        /// <summary>Lấy khách hàng theo ID</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound(new { message = "Không tìm thấy khách hàng" }) : Ok(result);
        }

        /// <summary>Tạo khách hàng mới (ADMIN tạo trực tiếp)</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật khách hàng</summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto dto)
        {
            // Customer chỉ sửa thông tin của chính mình
            var role = User.GetRole();
            if (role == "CUSTOMER")
            {
                var myCustomer = await _service.GetByUserIdAsync(User.GetUserId());
                if (myCustomer == null || myCustomer.Id != id)
                    return Forbid();
            }

            try
            {
                var result = await _service.UpdateAsync(id, dto);
                return result == null ? NotFound(new { message = "Không tìm thấy khách hàng" }) : Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Xóa (vô hiệu hóa) khách hàng - ADMIN</summary>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok(new { message = "Đã vô hiệu hóa tài khoản" }) : NotFound(new { message = "Không tìm thấy khách hàng" });
        }
    }
}
