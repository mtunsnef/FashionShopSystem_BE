using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FashionShopSystem.Service.Services.UserService;
using FashionShopSystem.Service.DTOs.UserDto;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

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

		/// <summary>
		/// Get current user information based on JWT token
		/// </summary>
		/// <returns>Current user profile</returns>
		[HttpGet("me")]
		[Authorize]
		public async Task<IActionResult> GetCurrentUser()
		{
			try
			{
				Console.WriteLine("🔄 GetCurrentUser endpoint hit");
				
				// Debug: Print all claims
				Console.WriteLine("🔍 JWT Claims:");
				foreach (var claim in User.Claims)
				{
					Console.WriteLine($"   {claim.Type}: {claim.Value}");
				}

				// Get the current user ID from JWT token
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				Console.WriteLine($"📋 User ID from token: {userIdClaim}");
				
				if (string.IsNullOrEmpty(userIdClaim))
				{
					Console.WriteLine("❌ User ID claim is null or empty");
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				Console.WriteLine($"🔍 Calling UserService.GetUserByIdAsync with ID: {userIdClaim}");
				var user = await _userService.GetUserByIdAsync(userIdClaim);
				
				if (user == null)
				{
					Console.WriteLine("❌ User not found in database");
					return NotFound(new { message = "User not found" });
				}

				Console.WriteLine($"✅ User found: {user.Email} (ID: {user.UserId})");

				// Return user data without password hash
				return Ok(new
				{
					userId = user.UserId,
					fullName = user.FullName,
					email = user.Email,
					phone = user.Phone,
					address = user.Address,
					role = user.Role,
					createdAt = user.CreatedAt,
					isActive = user.IsActive
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Exception in GetCurrentUser: {ex.Message}");
				Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get user profile by ID (Admin only)
		/// </summary>
		/// <param name="id">User ID</param>
		/// <returns>User profile</returns>
		[HttpGet("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetUserById(string id)
		{
			try
			{
				var user = await _userService.GetUserByIdAsync(id);
				
				if (user == null)
				{
					return NotFound(new { message = "User not found" });
				}

				return Ok(new
				{
					userId = user.UserId,
					fullName = user.FullName,
					email = user.Email,
					phone = user.Phone,
					address = user.Address,
					role = user.Role,
					createdAt = user.CreatedAt,
					isActive = user.IsActive
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get all users (Admin only)
		/// </summary>
		/// <returns>List of all users</returns>
		[HttpGet]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				var users = await _userService.GetAllAsync();
				
				var userProfiles = users.Select(user => new
				{
					userId = user.UserId,
					fullName = user.FullName,
					email = user.Email,
					phone = user.Phone,
					address = user.Address,
					role = user.Role,
					createdAt = user.CreatedAt,
					isActive = user.IsActive
				});

				return Ok(userProfiles);
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Update current user profile (partial update)
		/// </summary>
		/// <param name="dto">Fields to update (only send fields that need to be changed)</param>
		/// <returns>Update result</returns>
		[HttpPatch("me")]
		[Authorize]
		public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto dto)
		{
			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				
				if (string.IsNullOrEmpty(userIdClaim))
				{
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				var result = await _userService.PatchAccount(userIdClaim, dto);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return Ok(new { message = result.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Update user by ID (Admin only - partial update)
		/// </summary>
		/// <param name="id">User ID</param>
		/// <param name="dto">Fields to update (only send fields that need to be changed)</param>
		/// <returns>Update result</returns>
		[HttpPatch("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto dto)
		{
			try
			{
				var result = await _userService.PatchAccount(id, dto);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return Ok(new { message = result.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Delete user by ID (Admin only)
		/// </summary>
		/// <param name="id">User ID</param>
		/// <returns>Delete result</returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeleteUser(string id)
		{
			try
			{
				var result = await _userService.DeleteAccount(id);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return Ok(new { message = result.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Create new user (Admin only)
		/// </summary>
		/// <param name="dto">User creation data</param>
		/// <returns>Creation result</returns>
		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreateUser([FromBody] CreateUpdateUserDto dto)
		{
			try
			{
				var result = await _userService.CreateAccount(dto);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return CreatedAtAction(nameof(GetUserById), new { id = result.Data }, new { message = result.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

				/// <summary>
		/// Change current user's password
		/// </summary>
		/// <param name="dto">Current password and new password</param>
		/// <returns>Change password result</returns>
		[HttpPatch("me/change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
		{
			try
			{
				var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				
				if (string.IsNullOrEmpty(userIdClaim))
				{
					return Unauthorized(new { message = "Invalid token: User ID not found" });
				}

				var result = await _userService.ChangePassword(userIdClaim, dto);
				
				if (!result.Success)
				{
					return BadRequest(new { message = result.Message });
				}

				return Ok(new { message = result.Message });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}

		/// <summary>
		/// Get JWT token information (for debugging purposes)
		/// </summary>
		/// <returns>Token claims</returns>
		[HttpGet("token-info")]
		[Authorize]
		public IActionResult GetTokenInfo()
		{
			try
			{ 
				var claims = User.Claims.Select(c => new { type = c.Type, value = c.Value });
				
				return Ok(new
				{
					claims = claims,
					userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
					email = User.FindFirst(ClaimTypes.Email)?.Value,
					role = User.FindFirst(ClaimTypes.Role)?.Value,
					name = User.FindFirst(ClaimTypes.Name)?.Value
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = "Internal server error", error = ex.Message });
			}
		}
	}
}
