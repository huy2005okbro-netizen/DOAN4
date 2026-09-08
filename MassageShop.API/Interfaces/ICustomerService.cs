using MassageShop.API.Models.DTOs.Customer;

namespace MassageShop.API.Interfaces
{
    public interface ICustomerService
    {
        Task<List<CustomerResponseDto>> GetAllAsync();
        Task<CustomerResponseDto?> GetByIdAsync(int id);
        Task<CustomerResponseDto> CreateAsync(CustomerCreateDto dto);
        Task<CustomerResponseDto?> UpdateAsync(int id, CustomerUpdateDto dto);
        Task<bool> DeleteAsync(int id);
        Task<CustomerResponseDto?> GetByUserIdAsync(int userId);
    }
}
