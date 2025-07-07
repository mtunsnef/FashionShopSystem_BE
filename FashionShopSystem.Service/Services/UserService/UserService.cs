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

		public async Task<ApiResponseDto<string>> CreateAccount(CreateUpdateUserDto dto)
		{
			Console.WriteLine($"🔍 UserService.CreateAccount called for email: {dto.Email}");
			
			try
			{
				// Check if user already exists
				var existingUser = await _userRepository.GetAccountByEmail(dto.Email);
				if (existingUser != null)
				{
					return new ApiResponseDto<string>(false, null, 400, "Email already exists.");
				}

				var user = new User
				{
					FullName = dto.FullName,
					Email = dto.Email,
					PasswordHash = dto.Password != null ? BCrypt.Net.BCrypt.HashPassword(dto.Password) : null,
					Phone = dto.PhoneNumber,
					Role = dto.RoleId == 1 ? "Admin" : "Customer", // Convert RoleId to Role string
					CreatedAt = DateTime.UtcNow,
					IsActive = true
				};

				await _userRepository.AddAsync(user);
				Console.WriteLine($"✅ User created successfully with ID: {user.UserId}");
				
				return new ApiResponseDto<string>(true, user.UserId.ToString(), 200, "User created successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error creating user: {ex.Message}");
				return new ApiResponseDto<string>(false, null, 500, "Error creating user.");
			}
		}

		public async Task<ApiResponseDto<string>> ChangePassword(string id, ChangePasswordDto dto)
		{
			Console.WriteLine($"🔍 UserService.ChangePassword called for ID: {id}");
			
			try
			{
				if (!int.TryParse(id, out var userId))
				{
					return new ApiResponseDto<string>(false, null, 400, "Invalid user ID format.");
				}

				if (string.IsNullOrEmpty(dto.CurrentPassword))
				{
					return new ApiResponseDto<string>(false, null, 400, "Current password is required.");
				}

				if (string.IsNullOrEmpty(dto.NewPassword))
				{
					return new ApiResponseDto<string>(false, null, 400, "New password is required.");
				}

				var user = await _userRepository.GetByIdAsync(userId);
				if (user == null)
				{
					return new ApiResponseDto<string>(false, null, 404, "User not found.");
				}

				// Verify current password
				if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
				{
					Console.WriteLine($"❌ Invalid current password for user {id}");
					return new ApiResponseDto<string>(false, null, 400, "Current password is incorrect.");
				}

				Console.WriteLine($"✅ Current password verified for user {id}");

				// Hash the new password before storing
				user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
				Console.WriteLine($"🔄 Password changed for user {id}");

				await _userRepository.UpdateAsync(user);
				Console.WriteLine($"✅ Password updated successfully for user {id}");
				
				return new ApiResponseDto<string>(true, null, 200, "Password changed successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error changing password for user {id}: {ex.Message}");
				return new ApiResponseDto<string>(false, null, 500, "Error changing password.");
			}
		}

		public async Task<ApiResponseDto<string>> DeleteAccount(string id)
		{
			Console.WriteLine($"🔍 UserService.DeleteAccount called for ID: {id}");
			
			try
			{
				if (!int.TryParse(id, out var userId))
				{
					return new ApiResponseDto<string>(false, null, 400, "Invalid user ID format.");
				}

				var user = await _userRepository.GetByIdAsync(userId);
				if (user == null)
				{
					return new ApiResponseDto<string>(false, null, 404, "User not found.");
				}

				await _userRepository.DeleteAsync(user);
				Console.WriteLine($"✅ User {id} deleted successfully");
				
				return new ApiResponseDto<string>(true, null, 200, "User deleted successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error deleting user {id}: {ex.Message}");
				return new ApiResponseDto<string>(false, null, 500, "Error deleting user.");
			}
		}

		public async Task<User?> GetAccountByEmail(string email)
		{
			Console.WriteLine($"🔍 UserService.GetAccountByEmail called with email: {email}");
			return await _userRepository.GetAccountByEmail(email);
		}

		public async Task<List<User>> GetAllAsync()
		{
			Console.WriteLine("🔍 UserService.GetAllAsync called");
			return await _userRepository.GetAllAsync();
		}

		public async Task<User?> GetUserByIdAsync(string id)
		{
			Console.WriteLine($"🔍 UserService.GetUserByIdAsync called with ID: {id}");
			
			if (int.TryParse(id, out var userId))
			{
				Console.WriteLine($"✅ Successfully parsed ID {id} to int {userId}");
				var user = await _userRepository.GetByIdAsync(userId);
				Console.WriteLine($"📋 Repository returned: {(user != null ? $"User found - {user.Email}" : "User not found")}");
				return user;
			}
			
			Console.WriteLine($"❌ Failed to parse ID '{id}' to integer");
			return null;
		}

		public async Task<ApiResponseDto<string>> UpdateAccount(string id, CreateUpdateUserDto dto)
		{
			Console.WriteLine($"🔍 UserService.UpdateAccount called for ID: {id}");
			
			try
			{
				if (!int.TryParse(id, out var userId))
				{
					return new ApiResponseDto<string>(false, null, 400, "Invalid user ID format.");
				}

				var user = await _userRepository.GetByIdAsync(userId);
				if (user == null)
				{
					return new ApiResponseDto<string>(false, null, 404, "User not found.");
				}

				// Update user properties
				user.FullName = dto.FullName ?? user.FullName;
				user.Email = dto.Email ?? user.Email;
				user.Phone = dto.PhoneNumber ?? user.Phone;
				user.Role = dto.RoleId == 1 ? "Admin" : "Customer";
				
				// Only update password if provided
				if (!string.IsNullOrEmpty(dto.Password))
				{
					user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
				}

				await _userRepository.UpdateAsync(user);
				Console.WriteLine($"✅ User {id} updated successfully");
				
				return new ApiResponseDto<string>(true, null, 200, "User updated successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error updating user {id}: {ex.Message}");
				return new ApiResponseDto<string>(false, null, 500, "Error updating user.");
			}
		}

		public async Task<ApiResponseDto<string>> PatchAccount(string id, UpdateUserDto dto)
		{
			Console.WriteLine($"🔍 UserService.PatchAccount called for ID: {id}");
			Console.WriteLine($"📋 Fields to update: FullName={dto.FullName}, Email={dto.Email}, Phone={dto.PhoneNumber}, RoleId={dto.RoleId}, Password={(dto.Password != null ? "***" : "null")}");
			
			try
			{
				if (!int.TryParse(id, out var userId))
				{
					return new ApiResponseDto<string>(false, null, 400, "Invalid user ID format.");
				}

				var user = await _userRepository.GetByIdAsync(userId);
				if (user == null)
				{
					return new ApiResponseDto<string>(false, null, 404, "User not found.");
				}

				Console.WriteLine($"📋 Current user data: FullName={user.FullName}, Email={user.Email}, Phone={user.Phone}, Role={user.Role}");

				bool hasChanges = false;

				// Only update provided fields (true partial update)
				if (dto.FullName != null && dto.FullName != user.FullName)
				{
					Console.WriteLine($"🔄 Updating FullName: {user.FullName} -> {dto.FullName}");
					user.FullName = dto.FullName;
					hasChanges = true;
				}
				
				if (dto.Email != null && dto.Email != user.Email)
				{
					Console.WriteLine($"🔄 Updating Email: {user.Email} -> {dto.Email}");
					user.Email = dto.Email;
					hasChanges = true;
				}
				
				if (dto.PhoneNumber != null && dto.PhoneNumber != user.Phone)
				{
					Console.WriteLine($"🔄 Updating Phone: {user.Phone} -> {dto.PhoneNumber}");
					user.Phone = dto.PhoneNumber;
					hasChanges = true;
				}
				
				if (dto.RoleId.HasValue)
				{
					var newRole = dto.RoleId == 1 ? "Admin" : "Customer";
					if (newRole != user.Role)
					{
						Console.WriteLine($"🔄 Updating Role: {user.Role} -> {newRole}");
						user.Role = newRole;
						hasChanges = true;
					}
				}
				
				// Only update password if provided
				if (!string.IsNullOrEmpty(dto.Password))
				{
					Console.WriteLine("🔄 Updating password");
					user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
					hasChanges = true;
				}

				if (hasChanges)
				{
					Console.WriteLine("💾 Changes detected, saving to database...");
					await _userRepository.UpdateAsync(user);
					Console.WriteLine($"✅ User {id} patched successfully");
				}
				else
				{
					Console.WriteLine("ℹ️ No changes detected, skipping database update");
				}
				
				return new ApiResponseDto<string>(true, null, 200, "User updated successfully.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"❌ Error patching user {id}: {ex.Message}");
				Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
				return new ApiResponseDto<string>(false, null, 500, "Error updating user.");
			}
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
