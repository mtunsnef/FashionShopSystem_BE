using System.Text.Json;
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
		private readonly IConfiguration _configuration;
		public Authenticate(IUserService userService, IConfiguration configuration)
		{
			_userService = userService;
			_configuration = configuration;
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

		[HttpGet("google/callback")]
		public async Task<IActionResult> GoogleCallback([FromQuery] string code)
		{
			Console.WriteLine("🔄 GoogleCallback endpoint hit");
			Console.WriteLine($"📦 Received code: {code}");

			if (string.IsNullOrEmpty(code))
			{
				Console.WriteLine("❌ Missing authorization code.");
				return BadRequest(new { error = "Missing authorization code." });
			}

			using var httpClient = new HttpClient();

			var requestBody = new Dictionary<string, string>
	{
		{ "code", code },
		{ "client_id", _configuration["AuthSettings:Google:ClientId"]! },
		{ "client_secret", _configuration["AuthSettings:Google:ClientSecret"]! },
		{ "redirect_uri", "https://localhost:7242/api/Authenticate/google/callback" },
		{ "grant_type", "authorization_code" }
	};

			Console.WriteLine("📤 Sending token exchange request to Google...");

			var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(requestBody));
			var tokenJson = await tokenResponse.Content.ReadAsStringAsync();

			Console.WriteLine($"📥 Google token response: {tokenJson}");

			if (!tokenResponse.IsSuccessStatusCode)
			{
				Console.WriteLine("❌ Token exchange failed.");
				return Ok(new
				{
					success = false,
					error = "Failed to exchange code for token.",
					response = tokenJson
				});
			}

			using var doc = JsonDocument.Parse(tokenJson);
			if (!doc.RootElement.TryGetProperty("id_token", out var idTokenElement))
			{
				Console.WriteLine("❌ id_token not found in response.");
				return Ok(new
				{
					success = false,
					error = "No id_token returned from Google.",
					response = tokenJson
				});
			}

			var idToken = idTokenElement.GetString();
			Console.WriteLine($"✅ Received id_token: {idToken}");

			if (string.IsNullOrEmpty(idToken))
			{
				Console.WriteLine("❌ id_token is null or empty.");
				return Ok(new
				{
					success = false,
					error = "Invalid id_token from Google.",
					response = tokenJson
				});
			}

			// Step 2: Reuse your login logic
			var dto = new GoogleLoginDto { IdToken = idToken };
			Console.WriteLine("📨 Passing id_token to _userService.GoogleLoginAsync...");
			var result = await _userService.GoogleLoginAsync(dto);

			if (!result.Success)
			{
				Console.WriteLine($"❌ GoogleLoginAsync failed: {result.Message}");
				return Ok(new { success = false, error = result.Message });
			}

			Console.WriteLine($"✅ Google login successful. Returning token...");
			return Ok(new { success = true, token = result.Data, message = result.Message });
			//return Redirect("FRONTEND")
		}

	}
}
