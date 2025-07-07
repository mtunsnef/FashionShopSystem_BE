using FashionShopSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionShopSystem.Infrastructure.Repositories.OrderRepo
{
	public class OrderRepository : RepositoryBase<Order>, IOrderRepository
	{
		public OrderRepository(FashionShopContext context) : base(context)
		{
		}

		public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
		{
			return await _dbSet
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
				.Include(o => o.User)
				.Where(o => o.UserId == userId)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();
		}

		public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
		{
			return await _dbSet
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
				.Include(o => o.User)
				.FirstOrDefaultAsync(o => o.OrderId == orderId);
		}

		public async Task<List<Order>> GetOrdersWithDetailsAsync()
		{
			return await _dbSet
				.Include(o => o.OrderDetails)
					.ThenInclude(od => od.Product)
				.Include(o => o.User)
				.OrderByDescending(o => o.OrderDate)
				.ToListAsync();
		}
	}
} 