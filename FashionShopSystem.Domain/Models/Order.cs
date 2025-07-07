namespace FashionShopSystem.Domain.Models;

public partial class Order
{
	public int OrderId { get; set; }

	public int? UserId { get; set; }

	public DateTime? OrderDate { get; set; }

	public decimal? TotalAmount { get; set; }

	public string? PaymentStatus { get; set; }

	public string? DeliveryStatus { get; set; }

	public string? ShippingAddress { get; set; }

	public string? Email { get; set; }

	public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

	public virtual User? User { get; set; }
}
