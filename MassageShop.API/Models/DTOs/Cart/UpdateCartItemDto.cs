using System.ComponentModel.DataAnnotations;

namespace MassageShop.API.Models.DTOs.Cart
{
    public class UpdateCartItemDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; }
    }
}
