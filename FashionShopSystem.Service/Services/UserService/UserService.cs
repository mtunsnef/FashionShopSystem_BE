using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories.UserRepo;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.UserDto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FashionShopSystem.Service.Services.UserService
{
	public class UserService : IUserService
	{
		private readonly IUserRepository _userRepository;
		private readonly IConfiguration _configuration;

		public UserService(IUserRepository userRepository, IConfiguration configuration)
		{
			_userRepository = userRepository;
			_configuration = configuration;
			

		}

		public async Task<ApiResponseDto<string>> RegisterAsync(RegisterUserDto dto)
		{
			var existingUser = await _userRepository.GetAccountByEmail(dto.Email);
			if (existingUser != null)
			{
				return new ApiResponseDto<string>(false, null, 400, "Email already exists.");
			}

			var user = new User
			{
				FullName = dto.FullName,
				Email = dto.Email,
				PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
				Role = "Customer",
				CreatedAt = DateTime.UtcNow,
				IsActive = true
			};
			await _userRepository.AddAsync(user);

			var token = GenerateJwtToken(user);
			return new ApiResponseDto<string>(true, token, 200, "Registration successful.");
		}

		private string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
				new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
				new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
				new Claim("FullName", user.FullName ?? string.Empty)
			};
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddDays(7),
				signingCredentials: creds
			);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public Task<ApiResponseDto<string>> CreateAccount(CreateUpdateUserDto dto)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponseDto<string>> DeleteAccount(string id)
		{
			throw new NotImplementedException();
		}

		public Task<User?> GetAccountByEmail(string email)
		{
			throw new NotImplementedException();
		}

		public Task<List<User>> GetAllAsync()
		{
			throw new NotImplementedException();
		}

		public Task<User?> GetUserByIdAsync(string id)
		{
			throw new NotImplementedException();
		}

		public Task<ApiResponseDto<string>> UpdateAccount(string id, CreateUpdateUserDto dto)
		{
			throw new NotImplementedException();
		}

		public async Task<ApiResponseDto<string>> LoginAsync(LoginUserDto dto)
		{
			var user = await _userRepository.GetAccountByEmail(dto.Email);

			if (user == null || !user.IsActive.GetValueOrDefault())
			{
				return new ApiResponseDto<string>(false, null, 400, "Invalid email or account is inactive.");
			}
			if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
			{
				return new ApiResponseDto<string>(false, null, 400, "Invalid password.");
			}
			var token = GenerateJwtToken(user);
			return new ApiResponseDto<string>(true, token, 200, "Login successful.");
		}

		public async Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto)
		{
			var user = await _userRepository.GetAccountByEmail(dto.Email);
			if (user == null || !user.IsActive.GetValueOrDefault())
			{
				return new ApiResponseDto<string>(false, null, 400, "User not found or inactive.");
			}
			if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 6)
			{
				return new ApiResponseDto<string>(false, null, 400, "Password must be at least 6 characters.");
			}
			user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
			await _userRepository.UpdateAsync(user);
			return new ApiResponseDto<string>(true, null, 200, "Password updated successfully.");
		}

		public async Task<ApiResponseDto<string>> GoogleLoginAsync(GoogleLoginDto dto)
		{
			// Validate Google ID token
			var validPayload = await ValidateGoogleToken(dto.IdToken);
			if (validPayload == null)
			{
				return new ApiResponseDto<string>(false, null, 400, "Invalid Google token.");
			}

			// Check if user exists
			var user = await _userRepository.GetAccountByEmail(validPayload.Email);
			bool isNewUser = false;
			if (user == null)
			{
				user = new User
				{
					FullName = validPayload.Name,
					Email = validPayload.Email,
					Role = "Customer",
					CreatedAt = DateTime.UtcNow,
					IsActive = true
				};
				await _userRepository.AddAsync(user);
				isNewUser = true;
			}
			else if (!user.IsActive.GetValueOrDefault())
			{
				return new ApiResponseDto<string>(false, null, 400, "User account is inactive.");
			}

			var token = GenerateJwtToken(user);

			// Send welcome email if new user
			if (isNewUser)
			{
				var mailService = new MailService(_configuration);
				await mailService.SendEmailAsync(user.Email!, "Welcome to ShopClothes", $"<h2>Welcome {user.FullName} to our shop!</h2><p>Thank you for signing in with Google.Hope you will enjoy our products!</p>");
			}

			return new ApiResponseDto<string>(true, token, 200, "Google login successful.");
		}

		private async Task<Google.Apis.Auth.GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string idToken)
		{
			try
			{
				var clientId = _configuration["AuthSettings:Google:ClientId"];
				var settings = new Google.Apis.Auth.GoogleJsonWebSignature.ValidationSettings()
				{
					Audience = new[] { clientId }
				};
				var payload = await Google.Apis.Auth.GoogleJsonWebSignature.ValidateAsync(idToken, settings);
				return payload;
			}
			catch
			{
				return null;
			}
		}
	}
}
