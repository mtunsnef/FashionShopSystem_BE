namespace FashionShopSystem.Service.DTOs.OrderDto
{
	public class UpdateOrderDto
	{
		public string? PaymentStatus { get; set; }
		public string? DeliveryStatus { get; set; }
		public string? ShippingAddress { get; set; }
		public string? Email { get; set; }
	}
} 