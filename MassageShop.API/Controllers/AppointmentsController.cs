using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Appointment;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;
        private readonly ICustomerService _customerService;

        public AppointmentsController(IAppointmentService service, ICustomerService customerService)
        {
            _service = service;
            _customerService = customerService;
        }

        /// <summary>Lấy tất cả lịch hẹn - ADMIN, EMPLOYEE. Filter tùy chọn theo status và date</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] AppointmentStatus? status,
            [FromQuery] DateTime? date)
        {
            var list = await _service.GetAllAsync();

            if (status.HasValue)
                list = list.Where(a => a.Status == status.Value).ToList();

            if (date.HasValue)
                list = list.Where(a => a.AppointmentDate.Date == date.Value.Date).ToList();

            return Ok(list);
        }

        /// <summary>Lấy lịch hẹn theo khách hàng ID - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId) =>
            Ok(await _service.GetByCustomerIdAsync(customerId));

        /// <summary>Lấy lịch hẹn hôm nay - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("today")]
        public async Task<IActionResult> GetToday()
        {
            var list = await _service.GetAllAsync();
            var today = list.Where(a => a.AppointmentDate.Date == DateTime.UtcNow.Date).ToList();
            return Ok(today);
        }

        /// <summary>Lấy lịch hẹn sắp tới - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpGet("upcoming")]
        public async Task<IActionResult> GetUpcoming()
        {
            var list = await _service.GetAllAsync();
            var upcoming = list
                .Where(a => a.AppointmentDate.Date >= DateTime.UtcNow.Date
                         && a.Status != AppointmentStatus.CANCELLED
                         && a.Status != AppointmentStatus.COMPLETED)
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .ToList();
            return Ok(upcoming);
        }

        /// <summary>Lấy lịch hẹn của tôi - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMy([FromQuery] AppointmentStatus? status)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            var list = await _service.GetByCustomerIdAsync(customer.Id);

            if (status.HasValue)
                list = list.Where(a => a.Status == status.Value).ToList();

            return Ok(list);
        }

        /// <summary>Lấy lịch hẹn theo ID</summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            // CUSTOMER chỉ xem lịch của mình
            if (User.GetRole() == "CUSTOMER")
            {
                var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
                if (customer == null || result.CustomerId != customer.Id) return Forbid();
            }

            return Ok(result);
        }

        /// <summary>Đặt lịch - CUSTOMER</summary>
        [Authorize(Roles = "CUSTOMER")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDto dto)
        {
            var customer = await _customerService.GetByUserIdAsync(User.GetUserId());
            if (customer == null) return NotFound(new { message = "Không tìm thấy thông tin khách hàng" });

            try
            {
                var result = await _service.CreateAsync(customer.Id, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>ADMIN/EMPLOYEE đặt lịch cho khách hàng</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPost("for-customer/{customerId}")]
        public async Task<IActionResult> CreateForCustomer(int customerId, [FromBody] AppointmentCreateDto dto)
        {
            try
            {
                var result = await _service.CreateAsync(customerId, dto);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>Cập nhật lịch hẹn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentUpdateDto dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound(new { message = "Không tìm thấy lịch hẹn" }) : Ok(result);
        }

        /// <summary>Cập nhật trạng thái lịch hẹn - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
        {
            var dto = new AppointmentUpdateDto { Status = req.Status };
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Xác nhận lịch hẹn (CONFIRMED) - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> Confirm(int id)
        {
            var dto = new AppointmentUpdateDto { Status = AppointmentStatus.CONFIRMED };
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Check-in khách (CHECKED_IN) - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/checkin")]
        public async Task<IActionResult> CheckIn(int id)
        {
            var dto = new AppointmentUpdateDto { Status = AppointmentStatus.CHECKED_IN };
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Bắt đầu thực hiện dịch vụ (IN_PROGRESS) - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/start")]
        public async Task<IActionResult> Start(int id)
        {
            var dto = new AppointmentUpdateDto { Status = AppointmentStatus.IN_PROGRESS };
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Hoàn thành dịch vụ (COMPLETED) - ADMIN, EMPLOYEE</summary>
        [Authorize(Roles = "ADMIN,EMPLOYEE")]
        [HttpPatch("{id}/complete")]
        public async Task<IActionResult> Complete(int id)
        {
            var dto = new AppointmentUpdateDto { Status = AppointmentStatus.COMPLETED };
            var result = await _service.UpdateAsync(id, dto);
            return result == null ? NotFound() : Ok(result);
        }

        /// <summary>Hủy lịch hẹn - CUSTOMER/ADMIN/EMPLOYEE</summary>
        [HttpDelete("{id}/cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var result = await _service.CancelAsync(id, User.GetUserId(), User.GetRole());
                return result
                    ? Ok(new { message = "Đã hủy lịch hẹn" })
                    : NotFound(new { message = "Không tìm thấy lịch hẹn" });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }

    public record UpdateStatusRequest(AppointmentStatus Status);
}
