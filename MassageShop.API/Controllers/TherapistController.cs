using MassageShop.API.Helpers;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Appointment;
using MassageShop.API.Models.DTOs.Customer;
using MassageShop.API.Models.DTOs.Employee;
using MassageShop.API.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MassageShop.API.Controllers
{
    /// <summary>
    /// Controller dành riêng cho Kỹ thuật viên (Therapist/Employee tự quản lý)
    /// Route: /api/therapist
    /// </summary>
    [ApiController]
    [Route("api/therapist")]
    [Authorize(Roles = "EMPLOYEE")]
    public class TherapistController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAppointmentService _appointmentService;
        private readonly ICustomerService _customerService;

        public TherapistController(
            IEmployeeService employeeService,
            IAppointmentService appointmentService,
            ICustomerService customerService)
        {
            _employeeService = employeeService;
            _appointmentService = appointmentService;
            _customerService = customerService;
        }

        // =============================================
        // PROFILE
        // =============================================

        /// <summary>Lấy thông tin cá nhân của kỹ thuật viên đang đăng nhập</summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetMyProfile()
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });
            return Ok(emp);
        }

        /// <summary>Cập nhật thông tin cá nhân (FullName, Phone)</summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TherapistUpdateProfileDto dto)
        {
            var userId = User.GetUserId();
            var updateDto = new EmployeeUpdateDto
            {
                FullName = dto.FullName,
                Phone = dto.Phone
            };
            try
            {
                var result = await _employeeService.UpdateByUserIdAsync(userId, updateDto);
                if (result == null)
                    return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>Đổi mật khẩu</summary>
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                return BadRequest(new { message = "Mật khẩu xác nhận không khớp" });

            if (dto.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới tối thiểu 6 ký tự" });

            try
            {
                var success = await _employeeService.ChangePasswordAsync(User.GetUserId(), dto.CurrentPassword, dto.NewPassword);
                if (!success)
                    return NotFound(new { message = "Không tìm thấy tài khoản" });
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // =============================================
        // LỊCH LÀM VIỆC / LỊCH MASSAGE
        // =============================================

        /// <summary>Lấy lịch massage của mình — filter theo date, status, fromDate, toDate</summary>
        [HttpGet("appointments")]
        public async Task<IActionResult> GetMyAppointments(
            [FromQuery] string? status,
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var query = new AppointmentQueryParams
            {
                Status = status,
                Date = date,
                FromDate = fromDate,
                ToDate = toDate
            };

            var list = await _appointmentService.GetByEmployeeIdAsync(emp.Id, query);
            return Ok(list);
        }

        /// <summary>Lấy lịch hôm nay của mình</summary>
        [HttpGet("appointments/today")]
        public async Task<IActionResult> GetTodayAppointments()
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var query = new AppointmentQueryParams { Date = DateTime.UtcNow.Date };
            var list = await _appointmentService.GetByEmployeeIdAsync(emp.Id, query);
            return Ok(list);
        }

        /// <summary>Lấy lịch sắp tới của mình (từ hôm nay, chưa hủy/chưa xong)</summary>
        [HttpGet("appointments/upcoming")]
        public async Task<IActionResult> GetUpcomingAppointments()
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var query = new AppointmentQueryParams { FromDate = DateTime.UtcNow.Date };
            var list = await _appointmentService.GetByEmployeeIdAsync(emp.Id, query);

            var upcoming = list
                .Where(a => a.Status != AppointmentStatus.CANCELLED && a.Status != AppointmentStatus.COMPLETED)
                .ToList();

            return Ok(upcoming);
        }

        /// <summary>Lấy lịch sử đã thực hiện (COMPLETED)</summary>
        [HttpGet("appointments/history")]
        public async Task<IActionResult> GetAppointmentHistory([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var query = new AppointmentQueryParams
            {
                Status = "COMPLETED",
                FromDate = fromDate,
                ToDate = toDate
            };
            var list = await _appointmentService.GetByEmployeeIdAsync(emp.Id, query);
            return Ok(list);
        }

        /// <summary>Xem chi tiết một lịch hẹn (phải là của mình)</summary>
        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> GetAppointmentDetail(int id)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var appt = await _appointmentService.GetByIdAsync(id);
            if (appt == null)
                return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            if (appt.EmployeeId != emp.Id)
                return Forbid();

            return Ok(appt);
        }

        /// <summary>Bắt đầu thực hiện dịch vụ → IN_PROGRESS</summary>
        [HttpPatch("appointments/{id}/start")]
        public async Task<IActionResult> StartService(int id)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var appt = await _appointmentService.GetByIdAsync(id);
            if (appt == null)
                return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            if (appt.EmployeeId != emp.Id)
                return Forbid();

            if (appt.Status != AppointmentStatus.CONFIRMED && appt.Status != AppointmentStatus.CHECKED_IN)
                return BadRequest(new { message = $"Không thể bắt đầu lịch hẹn ở trạng thái {appt.Status}" });

            var dto = new AppointmentUpdateDto { Status = AppointmentStatus.IN_PROGRESS };
            var result = await _appointmentService.UpdateAsync(id, dto);
            return Ok(result);
        }

        /// <summary>Hoàn thành dịch vụ → COMPLETED</summary>
        [HttpPatch("appointments/{id}/complete")]
        public async Task<IActionResult> CompleteService(int id, [FromBody] CompleteServiceDto? body)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var appt = await _appointmentService.GetByIdAsync(id);
            if (appt == null)
                return NotFound(new { message = "Không tìm thấy lịch hẹn" });

            if (appt.EmployeeId != emp.Id)
                return Forbid();

            if (appt.Status != AppointmentStatus.IN_PROGRESS)
                return BadRequest(new { message = "Lịch hẹn chưa ở trạng thái đang thực hiện" });

            var dto = new AppointmentUpdateDto
            {
                Status = AppointmentStatus.COMPLETED,
                Note = body?.Note
            };
            var result = await _appointmentService.UpdateAsync(id, dto);
            return Ok(result);
        }

        // =============================================
        // KHÁCH HÀNG CỦA TÔI
        // =============================================

        /// <summary>Danh sách khách hàng đã từng được phục vụ bởi kỹ thuật viên này</summary>
        [HttpGet("customers")]
        public async Task<IActionResult> GetMyCustomers()
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            // Lấy tất cả appointment của mình → lấy distinct customer
            var allAppts = await _appointmentService.GetByEmployeeIdAsync(emp.Id);

            var customerIds = allAppts
                .Where(a => a.Status == AppointmentStatus.COMPLETED)
                .Select(a => a.CustomerId)
                .Distinct()
                .ToList();

            var customers = new List<CustomerResponseDto>();
            foreach (var cid in customerIds)
            {
                var c = await _customerService.GetByIdAsync(cid);
                if (c != null) customers.Add(c);
            }

            // Thêm thống kê số lần phục vụ
            var result = customers.Select(c => new
            {
                c.Id,
                c.FullName,
                c.Email,
                c.Phone,
                c.Gender,
                c.Address,
                c.LoyaltyPoints,
                TotalSessions = allAppts.Count(a => a.CustomerId == c.Id && a.Status == AppointmentStatus.COMPLETED),
                LastVisit = allAppts
                    .Where(a => a.CustomerId == c.Id && a.Status == AppointmentStatus.COMPLETED)
                    .OrderByDescending(a => a.AppointmentDate)
                    .Select(a => (DateTime?)a.AppointmentDate)
                    .FirstOrDefault()
            }).OrderByDescending(x => x.LastVisit).ToList();

            return Ok(result);
        }

        /// <summary>Xem chi tiết một khách hàng (chỉ khách đã từng phục vụ)</summary>
        [HttpGet("customers/{customerId}")]
        public async Task<IActionResult> GetCustomerDetail(int customerId)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var allAppts = await _appointmentService.GetByEmployeeIdAsync(emp.Id);
            var hasServed = allAppts.Any(a => a.CustomerId == customerId);
            if (!hasServed)
                return Forbid();

            var customer = await _customerService.GetByIdAsync(customerId);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy khách hàng" });

            var customerAppts = allAppts
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return Ok(new
            {
                Customer = customer,
                Appointments = customerAppts,
                TotalSessions = customerAppts.Count(a => a.Status == AppointmentStatus.COMPLETED)
            });
        }

        // =============================================
        // THỐNG KÊ CÔNG VIỆC
        // =============================================

        /// <summary>Thống kê công việc tổng hợp (dashboard)</summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics([FromQuery] int? month, [FromQuery] int? year)
        {
            var userId = User.GetUserId();
            var emp = await _employeeService.GetByUserIdAsync(userId);
            if (emp == null)
                return NotFound(new { message = "Không tìm thấy thông tin kỹ thuật viên" });

            var now = DateTime.UtcNow;
            var m = month ?? now.Month;
            var y = year ?? now.Year;

            var fromDate = new DateTime(y, m, 1, 0, 0, 0, DateTimeKind.Utc);
            var toDate = fromDate.AddMonths(1).AddDays(-1);

            var allAppts = await _appointmentService.GetByEmployeeIdAsync(emp.Id,
                new AppointmentQueryParams { FromDate = fromDate, ToDate = toDate });

            return Ok(new
            {
                Month = m,
                Year = y,
                TotalAppointments = allAppts.Count,
                Completed = allAppts.Count(a => a.Status == AppointmentStatus.COMPLETED),
                InProgress = allAppts.Count(a => a.Status == AppointmentStatus.IN_PROGRESS),
                Pending = allAppts.Count(a => a.Status == AppointmentStatus.PENDING || a.Status == AppointmentStatus.CONFIRMED),
                Cancelled = allAppts.Count(a => a.Status == AppointmentStatus.CANCELLED),
                TodayAppointments = allAppts.Count(a => a.AppointmentDate.Date == now.Date),
                UniqueCustomers = allAppts.Select(a => a.CustomerId).Distinct().Count()
            });
        }
    }

    // =============================================
    // Request DTOs (local, không cần file riêng)
    // =============================================
    public class TherapistUpdateProfileDto
    {
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class CompleteServiceDto
    {
        public string? Note { get; set; }
    }
}
