using FashionShopSystem.Domain.Models;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.UserDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.Services.UserService
{
    public interface IUserService
    {
        Task<User?> GetUserByIdAsync(string id);
        Task<List<User>> GetAllAsync();
        Task<ApiResponseDto<string>> CreateAccount(CreateUpdateUserDto dto);
        Task<ApiResponseDto<string>> UpdateAccount(string id, CreateUpdateUserDto dto);
        Task<ApiResponseDto<string>> DeleteAccount(string id);
        Task<User?> GetAccountByEmail(string email);
    }
}
