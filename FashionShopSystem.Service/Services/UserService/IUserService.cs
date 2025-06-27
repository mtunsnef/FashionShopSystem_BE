using FashionShopSystem.Domain.Models;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.UserDto;

namespace FashionShopSystem.Service.Services.UserService
{
	public interface IUserService
	{
		Task<User?> GetUserByIdAsync(string id);
		Task<List<User>> GetAllAsync();
		Task<ApiResponseDto<string>> CreateAccount(CreateUpdateUserDto dto);
		Task<ApiResponseDto<string>> UpdateAccount(string id, CreateUpdateUserDto dto);
		Task<ApiResponseDto<string>> PatchAccount(string id, UpdateUserDto dto);
		Task<ApiResponseDto<string>> ChangePassword(string id, ChangePasswordDto dto);
		Task<ApiResponseDto<string>> DeleteAccount(string id);
		Task<User?> GetAccountByEmail(string email);
		Task<ApiResponseDto<string>> RegisterAsync(RegisterUserDto dto);
		Task<ApiResponseDto<string>> LoginAsync(LoginUserDto dto);
		Task<ApiResponseDto<string>> ForgotPasswordAsync(ForgotPasswordDto dto);
		Task<ApiResponseDto<string>> GoogleLoginAsync(GoogleLoginDto dto);
	}
}
