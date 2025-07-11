using FashionShopSystem.Domain.Models;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.OrderDto;

namespace FashionShopSystem.Service.Services.OrderService
{
	public interface IOrderService
	{
		Task<Order?> GetOrderByIdAsync(int id);
		Task<List<Order>> GetAllOrdersAsync();
		Task<List<Order>> GetOrdersByUserIdAsync(int userId);
		Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto);
		Task<ApiResponseDto<OrderResponseDto>> UpdateOrderAsync(int id, UpdateOrderDto dto);
		Task<ApiResponseDto<string>> DeleteOrderAsync(int id);
		Task<OrderResponseDto?> GetOrderDetailsAsync(int id);
		Task<List<OrderResponseDto>> GetOrdersWithDetailsAsync();
		Task<List<OrderResponseDto>> GetUserOrdersWithDetailsAsync(int userId);
		Task<ApiResponseDto<OrderResponseDto>> CreateOrderFromCheckoutAsync(int userId, CreateOrderFromCheckoutDto dto);

    }
} 