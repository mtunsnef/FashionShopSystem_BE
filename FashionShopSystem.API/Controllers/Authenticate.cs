using FashionShopSystem.Service.DTOs.UserDto;
using FashionShopSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace FashionShopSystem.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class Authenticate : ControllerBase
	{
		private readonly IUserService _userService;
		public Authenticate(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
		{
			var result = await _userService.RegisterAsync(dto);
			if (!result.Success)
				return BadRequest(result.Message);
			return Ok(new { token = result.Data, message = result.Message });
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
		{
			var result = await _userService.LoginAsync(dto);
			if (!result.Success)
				return BadRequest(result.Message);
			return Ok(new { token = result.Data, message = result.Message });
		}

		[HttpPost("logout")]
		public IActionResult Logout()
		{

			return Ok(new { message = "Logout successful." });
		}

		[HttpPost("forgot-password")]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
		{
			if (dto.NewPassword != dto.ConfirmPassword)
				return BadRequest("Passwords do not match.");

			var result = await _userService.ForgotPasswordAsync(dto);
			if (!result.Success)
				return BadRequest(result.Message);
			return Ok(new { message = result.Message });
		}

		[HttpPost("google-login")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
		{
			var result = await _userService.GoogleLoginAsync(dto);
			if (!result.Success)
				return BadRequest(result.Message);
			return Ok(new { token = result.Data, message = result.Message });
		}
	}
}
