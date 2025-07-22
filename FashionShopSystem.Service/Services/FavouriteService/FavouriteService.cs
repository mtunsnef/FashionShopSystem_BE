using System.Security.Claims;
using System.Security.Claims;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.Services.UserService;

namespace FashionShopSystem.Service
{
    public class FavouriteService : IFavouriteService
    {
        private readonly IFavouriteRepo _favouriteRepo;
        private readonly IUserService _user;
        public FavouriteService(IUserService userService, IFavouriteRepo favouriteRepo, IUserService user)
        {
            _favouriteRepo = favouriteRepo;
            _user = user;
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
        public async Task<ApiResponseDto<List<FavouriteResponseDto>>> GetFavouritesByUserId(int userId)
        {
            var favourites = await _favouriteRepo.GetFavoritesByUserIdAsync(userId);

            var response = favourites.Select(f => new FavouriteResponseDto
            {
                FavoriteId = f.FavoriteId,
                UserId = f.UserId,
                ProductId = f.ProductId,
                CreatedAt = f.CreatedAt,
                Product = new ProductResponseDto
                {
                    ProductId = f.Product.ProductId,
                    ProductName = f.Product.ProductName,
                    ImageUrl = f.Product.ImageUrl,
                    Price = f.Product.Price
                }
            }).ToList();


            return new ApiResponseDto<List<FavouriteResponseDto>>(true, response, 200, "Get success");
        }


    }
}
