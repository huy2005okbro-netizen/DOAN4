using MassageShop.API.Data;
using MassageShop.API.Interfaces;
using MassageShop.API.Models.DTOs.Appointment;
using MassageShop.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace MassageShop.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _db;

        public AppointmentService(AppDbContext db) => _db = db;

        public async Task<List<AppointmentResponseDto>> GetAllAsync() =>
            await QueryWithIncludes().Select(a => MapToDto(a)).ToListAsync();

        public async Task<List<AppointmentResponseDto>> GetByCustomerIdAsync(int customerId) =>
            await QueryWithIncludes()
                .Where(a => a.CustomerId == customerId)
                .Select(a => MapToDto(a))
                .ToListAsync();

        public async Task<AppointmentResponseDto?> GetByIdAsync(int id)
        {
            var a = await QueryWithIncludes().FirstOrDefaultAsync(a => a.Id == id);
            return a == null ? null : MapToDto(a);
        }

        public async Task<AppointmentResponseDto> CreateAsync(int customerId, AppointmentCreateDto dto)
        {
            // Không đặt lịch trong quá khứ
            if (dto.AppointmentDate.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Không thể đặt lịch trong quá khứ");

            var service = await _db.Services.FindAsync(dto.ServiceId)
                ?? throw new KeyNotFoundException("Dịch vụ không tồn tại");

            var endTime = dto.StartTime.Add(TimeSpan.FromMinutes(service.Duration));

            // Kiểm tra trùng lịch nhân viên
            if (dto.EmployeeId.HasValue)
            {
                var empConflict = await _db.Appointments.AnyAsync(a =>
                    a.EmployeeId == dto.EmployeeId &&
                    a.AppointmentDate.Date == dto.AppointmentDate.Date &&
                    a.Status != AppointmentStatus.CANCELLED &&
                    a.StartTime < endTime && a.EndTime > dto.StartTime);

                if (empConflict)
                    throw new InvalidOperationException("Nhân viên đã có lịch trong khoảng thời gian này");
            }

            // Kiểm tra trùng lịch phòng
            if (dto.RoomId.HasValue)
            {
                var roomConflict = await _db.Appointments.AnyAsync(a =>
                    a.RoomId == dto.RoomId &&
                    a.AppointmentDate.Date == dto.AppointmentDate.Date &&
                    a.Status != AppointmentStatus.CANCELLED &&
                    a.StartTime < endTime && a.EndTime > dto.StartTime);

                if (roomConflict)
                    throw new InvalidOperationException("Phòng đã được đặt trong khoảng thời gian này");
            }

            var appointment = new Appointment
            {
                CustomerId = customerId,
                ServiceId = dto.ServiceId,
                EmployeeId = dto.EmployeeId,
                RoomId = dto.RoomId,
                AppointmentDate = dto.AppointmentDate,
                StartTime = dto.StartTime,
                EndTime = endTime,
                Note = dto.Note
            };

            _db.Appointments.Add(appointment);
            await _db.SaveChangesAsync();

            return (await GetByIdAsync(appointment.Id))!;
        }

        public async Task<AppointmentResponseDto?> UpdateAsync(int id, AppointmentUpdateDto dto)
        {
            var appointment = await _db.Appointments.FindAsync(id);
            if (appointment == null) return null;

            if (dto.EmployeeId.HasValue) appointment.EmployeeId = dto.EmployeeId;
            if (dto.RoomId.HasValue) appointment.RoomId = dto.RoomId;
            if (dto.AppointmentDate.HasValue) appointment.AppointmentDate = dto.AppointmentDate.Value;
            if (dto.StartTime.HasValue)
            {
                appointment.StartTime = dto.StartTime.Value;
                var service = await _db.Services.FindAsync(appointment.ServiceId);
                if (service != null)
                    appointment.EndTime = dto.StartTime.Value.Add(TimeSpan.FromMinutes(service.Duration));
            }
            if (dto.Status.HasValue) appointment.Status = dto.Status.Value;
            if (dto.Note != null) appointment.Note = dto.Note;

            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        public async Task<bool> CancelAsync(int id, int requesterId, string requesterRole)
        {
            var appointment = await _db.Appointments
                .Include(a => a.Customer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null) return false;

            // Customer chỉ hủy lịch của chính mình
            if (requesterRole == "CUSTOMER" && appointment.Customer.UserId != requesterId)
                throw new UnauthorizedAccessException("Không có quyền hủy lịch này");

            appointment.Status = AppointmentStatus.CANCELLED;
            await _db.SaveChangesAsync();
            return true;
        }

        private IQueryable<Appointment> QueryWithIncludes() =>
            _db.Appointments
                .Include(a => a.Customer).ThenInclude(c => c.User)
                .Include(a => a.Service)
                .Include(a => a.Employee).ThenInclude(e => e!.User)
                .Include(a => a.Room);

        private static AppointmentResponseDto MapToDto(Appointment a) => new()
        {
            Id = a.Id,
            CustomerId = a.CustomerId,
            CustomerName = a.Customer.User.FullName,
            ServiceId = a.ServiceId,
            ServiceName = a.Service.Name,
            ServicePrice = a.Service.Price,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee?.User.FullName,
            RoomId = a.RoomId,
            RoomNumber = a.Room?.RoomNumber,
            AppointmentDate = a.AppointmentDate,
            StartTime = a.StartTime,
            EndTime = a.EndTime,
            Status = a.Status,
            Note = a.Note,
            CreatedAt = a.CreatedAt
        };
    }
}
