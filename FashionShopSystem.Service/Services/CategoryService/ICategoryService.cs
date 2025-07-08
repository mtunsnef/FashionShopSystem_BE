using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Service.DTOs.ApiResponseDto;

namespace FashionShopSystem.Service
{
    public interface ICategoryService
    {
        public Task<ICollection<CategoryResponseDto>> getAllCategory();
        public Task<CategoryResponseDto> GetCategoryById(int id);
        public Task<ApiResponseDto<CategoryResponseDto>> UpdateCategory(UpdateCategoryDto dto);
        public Task<ApiResponseDto<CategoryResponseDto>> deteletCategory(int id);
        public Task<ApiResponseDto<CategoryResponseDto>> CreateCategory(CreateCategoryDto dto);
    }
}
