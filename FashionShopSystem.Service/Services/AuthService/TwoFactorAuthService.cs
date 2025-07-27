using FashionShopSystem.Domain.Exceptions.Http;
using FashionShopSystem.Infrastructure.Repositories.UserRepo;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs.AuthDto;
using FashionShopSystem.Service.Services.ConfigService;
using FashionShopSystem.Service.Services.UserService;
using OtpNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FashionShopSystem.Service.Services.AuthService
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly IUserRepository _userRepository;

        public TwoFactorAuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<ApiResponseDto<bool>> Check2FAEnabledAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");
            string message;
            if (user.Is2Faenabled)
            {
                message = "User has already enabled 2fa";
            }
            else
            {
                message = "User hasn't already enabled 2fa";
            } 
                
            return ApiResponseDto<bool>.SuccessResponse(user.Is2Faenabled, message);
        }

        public async Task<ApiResponseDto<TwoFaInitDto?>> GenerateSecretAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            var secretKey = KeyGeneration.GenerateRandomKey(20);
            var base32Secret = Base32Encoding.ToString(secretKey);

            user.Temp2FasecretKey = base32Secret;
            await _userRepository.UpdateAsync(user);

            string issuer = "FU News";
            string email = user.Email;
            string qrCodeUri = $"otpauth://totp/{HttpUtility.UrlEncode(issuer)}:{HttpUtility.UrlEncode(email)}?secret={base32Secret}&issuer={HttpUtility.UrlEncode(issuer)}&digits=6";

            var data = new TwoFaInitDto
            {
                SharedKey = base32Secret,
                QrCodeUri = qrCodeUri
            };
            return ApiResponseDto<TwoFaInitDto?>.SuccessResponse(data, "Generate Secret 2FA successfully");
        }
        public async Task<ApiResponseDto<bool>> VerifyCodeAsync(int userId, string code)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.Temp2FasecretKey))
                throw new NotFoundException("User not found or no temp key");

            var totp = new Totp(Base32Encoding.ToBytes(user.Temp2FasecretKey));
            bool isValid = totp.VerifyTotp(code, out _, VerificationWindow.RfcSpecifiedNetworkDelay);

            if (!isValid)
                return ApiResponseDto<bool>.FailResponse((int)HttpStatusCode.BadRequest, "Error to generate Secret 2FA");

            user.Is2Faenabled = true;
            user.TwoFactorSecretKey = user.Temp2FasecretKey;
            user.Temp2FasecretKey = null;
            await _userRepository.UpdateAsync(user);

            return ApiResponseDto<bool>.SuccessResponse(true, "Generate Secret 2FA successfully");
        }

        public async Task<ApiResponseDto<bool>> VerifyCodeAfterLoginAsync(int userId, string code)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || string.IsNullOrEmpty(user.TwoFactorSecretKey))
                throw new NotFoundException("No 2FA secret found");

            var totp = new Totp(Base32Encoding.ToBytes(user.TwoFactorSecretKey));
            var isValid = totp.VerifyTotp(code, out _);

            if (!isValid)
                return ApiResponseDto<bool>.FailResponse((int)HttpStatusCode.BadRequest, "Verify Code for 2FA not successfully");

            return ApiResponseDto<bool>.SuccessResponse(true, "Verify Code for 2FA successfully");
        }
    }
}
