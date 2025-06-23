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
    public class UserService : IUserService
    {
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
    }
}
