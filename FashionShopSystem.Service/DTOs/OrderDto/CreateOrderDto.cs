namespace FashionShopSystem.Service.DTOs.OrderDto
{
	public class CreateOrderDto
	{
		public string? ShippingAddress { get; set; }
		public string? Email { get; set; }
		public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
	}

	public class OrderItemDto
	{
		public int ProductId { get; set; }
		public int Quantity { get; set; }
		public decimal Price { get; set; }
	}
} 