using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Service.DTOs.ApiResponseDto;

namespace FashionShopSystem.Service
{
    public interface IFavouriteService 
    {
        public Task<ApiResponseDto<FavouriteResponseDto>> AddToFavourites(AddFavouriteDto dto, int UserId);
        public Task<ApiResponseDto<FavouriteResponseDto>> deleteFavourite(int id);
        public Task<ApiResponseDto<List<FavouriteResponseDto>>> GetFavouritesByUserId(int userId);
    }
}
