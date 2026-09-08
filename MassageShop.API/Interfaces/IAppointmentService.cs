using MassageShop.API.Models.DTOs.Appointment;

namespace MassageShop.API.Interfaces
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponseDto>> GetAllAsync();
        Task<List<AppointmentResponseDto>> GetByCustomerIdAsync(int customerId);
        Task<AppointmentResponseDto?> GetByIdAsync(int id);
        Task<AppointmentResponseDto> CreateAsync(int customerId, AppointmentCreateDto dto);
        Task<AppointmentResponseDto?> UpdateAsync(int id, AppointmentUpdateDto dto);
        Task<bool> CancelAsync(int id, int requesterId, string requesterRole);
    }
}
