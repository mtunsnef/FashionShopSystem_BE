using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OData.Query;
using FashionShopSystem.Service.Services.OrderService;
using FashionShopSystem.Service.DTOs.OrderDto;
using FashionShopSystem.Domain.Models;
using System.Security.Claims;

namespace FashionShopSystem.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		/// <summary>
		/// Get all orders with OData support (Admin only)
		/// </summary>
		/// <returns>List of orders with pagination, filtering, sorting support</returns>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		[EnableQuery(PageSize = 20, MaxTop = 100)]
		public async Task<IActionResult> GetAllOrders()
		{
			try
			{
				Console.WriteLine("🔍 OrderController.GetAllOrders called");
				var orders = await _orderService.GetOrdersWithDetailsAsync();
				return Ok(orders.AsQueryable());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in GetAllOrders: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get current user's orders with OData support
		/// </summary>
		/// <returns>List of current user's orders with pagination, filtering, sorting support</returns>
		[HttpGet("my-orders")]
		[Authorize]
		[EnableQuery(PageSize = 10, MaxTop = 50)]
		public async Task<IActionResult> GetMyOrders()
		{
			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				
				if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
				{
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				Console.WriteLine($"🔍 OrderController.GetMyOrders called for user: {userId}");
				var orders = await _orderService.GetUserOrdersWithDetailsAsync(userId);
				return Ok(orders.AsQueryable());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in GetMyOrders: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get order by ID
		/// </summary>
		/// <param name="id">Order ID</param>
		/// <returns>Order details</returns>
		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> GetOrderById(int id)
		{
			try
			{
				Console.WriteLine($"🔍 OrderController.GetOrderById called with ID: {id}");
				
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
				
				if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
				{
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				var order = await _orderService.GetOrderDetailsAsync(id);
				
				if (order == null)
				{
					return NotFound(new { message = "Order not found" });
				}

				// Users can only see their own orders, admins can see all orders
				if (userRole != "Admin" && order.UserId != userId)
				{
					return Forbid("You can only access your own orders");
				}

				return Ok(order);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in GetOrderById: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Create a new order for current user
		/// </summary>
		/// <param name="dto">Order creation data</param>
		/// <returns>Created order</returns>
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
		{
			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				
				if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
				{
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				Console.WriteLine($"🔍 OrderController.CreateOrder called for user: {userId}");
				
				if (dto.OrderItems == null || !dto.OrderItems.Any())
				{
					return BadRequest(new { message = "Order must contain at least one item" });
				}

				var result = await _orderService.CreateOrderAsync(userId, dto);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return CreatedAtAction(nameof(GetOrderById), new { id = result.Data.OrderId }, result.Data);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in CreateOrder: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Update order (Admin only for now)
		/// </summary>
		/// <param name="id">Order ID</param>
		/// <param name="dto">Order update data</param>
		/// <returns>Updated order</returns>
		[HttpPatch("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto dto)
		{
			try
			{
				Console.WriteLine($"🔍 OrderController.UpdateOrder called for ID: {id}");
				
				var result = await _orderService.UpdateOrderAsync(id, dto);
				
				if (!result.Success)
				{
					if (result.StatusCode == 404)
						return NotFound(new { message = result.Message });
					
					return BadRequest(new { message = result.Message });
				}

				return Ok(result.Data);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in UpdateOrder: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Delete order (Admin only)
		/// </summary>
		/// <param name="id">Order ID</param>
		/// <returns>Delete result</returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteOrder(int id)
		{
			try
			{
				Console.WriteLine($"🔍 OrderController.DeleteOrder called for ID: {id}");
				
				var result = await _orderService.DeleteOrderAsync(id);
				
				if (!result.Success)
				{
					if (result.StatusCode == 404)
						return NotFound(new { message = result.Message });
					
					return BadRequest(new { message = result.Message });
				}

				return Ok(new { message = result.Message });
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in DeleteOrder: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get user orders by user ID (Admin only)
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <returns>List of user's orders with pagination support</returns>
		[HttpGet("user/{userId}")]
		[Authorize(Roles = "Admin")]
		[EnableQuery(PageSize = 10, MaxTop = 50)]
		public async Task<IActionResult> GetOrdersByUserId(int userId)
		{
			try
			{
				Console.WriteLine($"🔍 OrderController.GetOrdersByUserId called for user: {userId}");
				var orders = await _orderService.GetUserOrdersWithDetailsAsync(userId);
				return Ok(orders.AsQueryable());
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error in GetOrdersByUserId: {ex.Message}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}
	}
} 