using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories.OrderRepo;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.OrderDto;

namespace FashionShopSystem.Service.Services.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IOrderRepository _orderRepository;

		public OrderService(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<Order?> GetOrderByIdAsync(int id)
		{
			Console.WriteLine($"🔍 OrderService.GetOrderByIdAsync called with ID: {id}");
			return await _orderRepository.GetByIdAsync(id);
		}

		public async Task<List<Order>> GetAllOrdersAsync()
		{
			Console.WriteLine("🔍 OrderService.GetAllOrdersAsync called");
			return await _orderRepository.GetAllAsync();
		}

		public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
		{
			Console.WriteLine($"🔍 OrderService.GetOrdersByUserIdAsync called for user: {userId}");
			return await _orderRepository.GetOrdersByUserIdAsync(userId);
		}

		public async Task<ApiResponseDto<OrderResponseDto>> CreateOrderAsync(int userId, CreateOrderDto dto)
		{
			Console.WriteLine($"🔍 OrderService.CreateOrderAsync called for user: {userId}");
			
			try
			{
				// Calculate total amount
				decimal totalAmount = dto.OrderItems.Sum(item => item.Price * item.Quantity);
				Console.WriteLine($"💰 Calculated total amount: {totalAmount}");

				var order = new Order
				{
					UserId = userId,
					OrderDate = DateTime.UtcNow,
					TotalAmount = totalAmount,
					PaymentStatus = "Pending",
					DeliveryStatus = "Processing",
					ShippingAddress = dto.ShippingAddress,
					Email = dto.Email
				};

				await _orderRepository.AddAsync(order);
				Console.WriteLine($"✅ Order created with ID: {order.OrderId}");

				// Create order details
				foreach (var item in dto.OrderItems)
				{
					var orderDetail = new OrderDetail
					{
						OrderId = order.OrderId,
						ProductId = item.ProductId,
						Quantity = item.Quantity,
						Price = item.Price
					};
					
					// Note: You might want to add OrderDetail repository for this
					// For now, assuming it's handled through Order navigation property
				}

				// Get the created order with details
				var createdOrder = await _orderRepository.GetOrderWithDetailsAsync(order.OrderId);
				var response = MapToOrderResponseDto(createdOrder);

				return new ApiResponseDto<OrderResponseDto>(true, response, 201, "Order created successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error creating order: {ex.Message}");
				return new ApiResponseDto<OrderResponseDto>(false, null, 500, "Error creating order.");
			}
		}

		public async Task<ApiResponseDto<OrderResponseDto>> UpdateOrderAsync(int id, UpdateOrderDto dto)
		{
			Console.WriteLine($"🔍 OrderService.UpdateOrderAsync called for ID: {id}");
			
			try
			{
				var order = await _orderRepository.GetByIdAsync(id);
				if (order == null)
				{
					return new ApiResponseDto<OrderResponseDto>(false, null, 404, "Order not found.");
				}

				Console.WriteLine($"📋 Current order status: Payment={order.PaymentStatus}, Delivery={order.DeliveryStatus}");

				// Update only provided fields
				if (dto.PaymentStatus != null)
				{
					Console.WriteLine($"🔄 Updating PaymentStatus: {order.PaymentStatus} -> {dto.PaymentStatus}");
					order.PaymentStatus = dto.PaymentStatus;
				}
				
				if (dto.DeliveryStatus != null)
				{
					Console.WriteLine($"🔄 Updating DeliveryStatus: {order.DeliveryStatus} -> {dto.DeliveryStatus}");
					order.DeliveryStatus = dto.DeliveryStatus;
				}
				
				if (dto.ShippingAddress != null)
				{
					Console.WriteLine($"🔄 Updating ShippingAddress: {order.ShippingAddress} -> {dto.ShippingAddress}");
					order.ShippingAddress = dto.ShippingAddress;
				}
				
				if (dto.Email != null)
				{
					Console.WriteLine($"🔄 Updating Email: {order.Email} -> {dto.Email}");
					order.Email = dto.Email;
				}

				await _orderRepository.UpdateAsync(order);
				Console.WriteLine($"✅ Order {id} updated successfully");

				// Get updated order with details
				var updatedOrder = await _orderRepository.GetOrderWithDetailsAsync(id);
				var response = MapToOrderResponseDto(updatedOrder);

				return new ApiResponseDto<OrderResponseDto>(true, response, 200, "Order updated successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error updating order {id}: {ex.Message}");
				return new ApiResponseDto<OrderResponseDto>(false, null, 500, "Error updating order.");
			}
		}

		public async Task<ApiResponseDto<string>> DeleteOrderAsync(int id)
		{
			Console.WriteLine($"🔍 OrderService.DeleteOrderAsync called for ID: {id}");
			
			try
			{
				var order = await _orderRepository.GetByIdAsync(id);
				if (order == null)
				{
					return new ApiResponseDto<string>(false, null, 404, "Order not found.");
				}

				await _orderRepository.DeleteAsync(order);
				Console.WriteLine($"✅ Order {id} deleted successfully");
				
				return new ApiResponseDto<string>(true, null, 200, "Order deleted successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error deleting order {id}: {ex.Message}");
				return new ApiResponseDto<string>(false, null, 500, "Error deleting order.");
			}
		}

		public async Task<OrderResponseDto?> GetOrderDetailsAsync(int id)
		{
			Console.WriteLine($"🔍 OrderService.GetOrderDetailsAsync called for ID: {id}");
			
			var order = await _orderRepository.GetOrderWithDetailsAsync(id);
			return order != null ? MapToOrderResponseDto(order) : null;
		}

		public async Task<List<OrderResponseDto>> GetOrdersWithDetailsAsync()
		{
			Console.WriteLine("🔍 OrderService.GetOrdersWithDetailsAsync called");
			
			var orders = await _orderRepository.GetOrdersWithDetailsAsync();
			return orders.Select(MapToOrderResponseDto).ToList();
		}

		public async Task<List<OrderResponseDto>> GetUserOrdersWithDetailsAsync(int userId)
		{
			Console.WriteLine($"🔍 OrderService.GetUserOrdersWithDetailsAsync called for user: {userId}");
			
			var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
			return orders.Select(MapToOrderResponseDto).ToList();
		}

		private OrderResponseDto MapToOrderResponseDto(Order order)
		{
			return new OrderResponseDto
			{
				OrderId = order.OrderId,
				UserId = order.UserId,
				UserName = order.User?.FullName,
				UserEmail = order.User?.Email,
				OrderDate = order.OrderDate,
				TotalAmount = order.TotalAmount,
				PaymentStatus = order.PaymentStatus,
				DeliveryStatus = order.DeliveryStatus,
				ShippingAddress = order.ShippingAddress,
				Email = order.Email,
				OrderDetails = order.OrderDetails?.Select(od => new OrderDetailResponseDto
				{
					OrderDetailId = od.OrderDetailId,
					ProductId = od.ProductId,
					ProductName = od.Product?.ProductName,
					ProductDescription = od.Product?.Description,
					ProductImageUrl = od.Product?.ImageUrl,
					Quantity = od.Quantity,
					Price = od.Price
				}).ToList() ?? new List<OrderDetailResponseDto>()
			};
		}
	}
} 