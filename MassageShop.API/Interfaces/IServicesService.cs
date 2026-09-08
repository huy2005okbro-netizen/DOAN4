using MassageShop.API.Models.DTOs.Service;

namespace MassageShop.API.Interfaces
{
    public interface IServicesService
    {
        Task<List<ServiceResponseDto>> GetAllAsync();
        Task<ServiceResponseDto?> GetByIdAsync(int id);
        Task<ServiceResponseDto> CreateAsync(ServiceCreateDto dto);
        Task<ServiceResponseDto?> UpdateAsync(int id, ServiceUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
