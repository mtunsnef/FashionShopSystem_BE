using FashionShopSystem.Domain.Models;

namespace FashionShopSystem.Infrastructure.Repositories.OrderRepo
{
	public interface IOrderRepository : IRepositoryBase<Order>
	{
		Task<List<Order>> GetOrdersByUserIdAsync(int userId);
		Task<Order?> GetOrderWithDetailsAsync(int orderId);
		Task<List<Order>> GetOrdersWithDetailsAsync();
	}
} 