using MassageShop.API.Models.DTOs.Employee;

namespace MassageShop.API.Interfaces
{
    public interface IEmployeeService
    {
        Task<List<EmployeeResponseDto>> GetAllAsync();
        Task<EmployeeResponseDto?> GetByIdAsync(int id);
        Task<EmployeeResponseDto?> GetByUserIdAsync(int userId);
        Task<EmployeeResponseDto> CreateAsync(EmployeeCreateDto dto);
        Task<EmployeeResponseDto?> UpdateAsync(int id, EmployeeUpdateDto dto);
        Task<EmployeeResponseDto?> UpdateByUserIdAsync(int userId, EmployeeUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
