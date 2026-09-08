namespace MassageShop.API.Models.DTOs.Cart
{
    public class CartResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<CartItemResponseDto> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CartItemResponseDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal SubTotal { get; set; }
    }
}
