
using Microsoft.AspNetCore.Authorization;
using FashionShopSystem.Service.Services.UserService;
using System.Security.Claims;
using FashionShopSystem.Service.DTOs.UserDto;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopSystem.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> Me()
		{
			// Get user id from claims
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim == null)
			{
				return Unauthorized(new { message = "User not authenticated." });
			}

			var userId = userIdClaim.Value;
			// Use GetUserByIdAsync from your user service
			var user = await _userService.GetUserByIdAsync(userId);
			if (user == null)
			{
				return NotFound(new { message = "User not found." });
			}
			return Ok(user);
		}
	}
}
