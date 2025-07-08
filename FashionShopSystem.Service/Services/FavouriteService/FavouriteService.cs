using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.Services.UserService;

namespace FashionShopSystem.Service
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepo _favouriteRepo;
        public FavouriteService(IUserService userService, IFavouriteRepo favouriteRepo)
        {
            _favouriteRepo = favouriteRepo;
        }
        public async Task<ApiResponseDto<FavouriteResponseDto>> AddToFavourites(AddFavouriteDto dto, int UserId)
        {
            var favourite = new Favorite
            {
                UserId = UserId,
                ProductId = dto.ProductId,
                CreatedAt = DateTime.UtcNow
            };
            await _favouriteRepo.AddAsync(favourite);
            var response = new FavouriteResponseDto
            {
                FavoriteId = favourite.FavoriteId,
                UserId = favourite.UserId,
                ProductId = favourite.ProductId,
                CreatedAt = favourite.CreatedAt
            };
            return new ApiResponseDto<FavouriteResponseDto>(true, response, 200, "create success");
        }
        public async Task<ApiResponseDto<FavouriteResponseDto>> deleteFavourite(int id)
        {
            var favourite = await _favouriteRepo.GetByIdAsync(id);
            if (favourite == null)
            {
                return new ApiResponseDto<FavouriteResponseDto>(false, null, 404, "Favourite not found");
            }
            await _favouriteRepo.DeleteAsync(favourite);
            var response = new FavouriteResponseDto
            {
                FavoriteId = favourite.FavoriteId,
                UserId = favourite.UserId,
                ProductId = favourite.ProductId,
                CreatedAt = favourite.CreatedAt
            };
            return new ApiResponseDto<FavouriteResponseDto>(true, response, 200, "delete success");
        }
        public async Task<ApiResponseDto<FavouriteResponseDto>> GetFavouriteById(int id)
        {
            var favourite = await _favouriteRepo.GetByIdAsync(id);
            if (favourite == null)
            {
                return new ApiResponseDto<FavouriteResponseDto>(false, null, 404, "Favourite not found");
            }
            var response = new FavouriteResponseDto
            {
                FavoriteId = favourite.FavoriteId,
                UserId = favourite.UserId,
                ProductId = favourite.ProductId,
                CreatedAt = favourite.CreatedAt
            };
            return new ApiResponseDto<FavouriteResponseDto>(true, response, 200, "get success");
        }
    }
}
