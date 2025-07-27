using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.AuthDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.Services.AuthService
{
    public interface ITwoFactorAuthService
    {
        Task<ApiResponseDto<TwoFaInitDto?>> GenerateSecretAsync(int userId);
        Task<ApiResponseDto<bool>> VerifyCodeAsync(int userId, string code);
        Task<ApiResponseDto<bool>> Check2FAEnabledAsync(int userId);
        Task<ApiResponseDto<bool>> VerifyCodeAfterLoginAsync(int userId, string code);
    }
}
