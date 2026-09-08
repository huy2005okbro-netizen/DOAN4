using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Customer
{
    public class CustomerUpdateDto
    {
        [MaxLength(100)]
        public string? FullName { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }
    }
}
