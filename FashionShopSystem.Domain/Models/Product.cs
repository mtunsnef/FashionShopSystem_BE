namespace FashionShopSystem.Domain.Models;

public partial class Product
{
	public int ProductId { get; set; }

	public string? ProductName { get; set; }

	public string? Description { get; set; }

	public decimal? Price { get; set; }

	public int? Stock { get; set; }

	public int? CategoryId { get; set; }

	public string? Brand { get; set; }

	public string? ImageUrl { get; set; }

	public DateTime? CreatedAt { get; set; }

	public bool? IsActive { get; set; }

	public virtual Category? Category { get; set; }

	public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();

	public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
