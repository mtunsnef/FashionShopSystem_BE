namespace FashionShopSystem.Service.DTOs.OrderDto
{
	public class OrderResponseDto
	{
		public int OrderId { get; set; }
		public int? UserId { get; set; }
		public string? UserName { get; set; }
		public string? UserEmail { get; set; }
		public DateTime? OrderDate { get; set; }
		public decimal? TotalAmount { get; set; }
		public string? PaymentStatus { get; set; }
		public string? DeliveryStatus { get; set; }
		public string? ShippingAddress { get; set; }
		public string? Email { get; set; }
		public List<OrderDetailResponseDto> OrderDetails { get; set; } = new List<OrderDetailResponseDto>();
	}

	public class OrderDetailResponseDto
	{
		public int OrderDetailId { get; set; }
		public int? ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? ProductDescription { get; set; }
		public string? ProductImageUrl { get; set; }
		public int? Quantity { get; set; }
		public decimal? Price { get; set; }
		public decimal? Subtotal => (Quantity ?? 0) * (Price ?? 0);
	}
} 