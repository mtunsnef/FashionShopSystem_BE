using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using Microsoft.OData.Edm;

namespace FashionShopSystem.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepo _categoryRepo;
        public CategoryService(ICategoryRepo categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }
        public async Task<ICollection<CategoryResponseDto>> getAllCategory()
        {
            var category = await _categoryRepo.GetAllAsync();
            var categoryResponse = category.Select(c => new CategoryResponseDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                Description = c.Description,
            }).ToList();
            return categoryResponse;
        }
        public async Task<CategoryResponseDto> GetCategoryById(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
            };
        }
        public async Task<ApiResponseDto<CategoryResponseDto>> CreateCategory(CreateCategoryDto dto)
        {
            var category = new Category
            {
                CategoryName = dto.Name,
                Description = dto.decription,
            };
            var Data = new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
            };
            await _categoryRepo.AddAsync(category);
            return new ApiResponseDto<CategoryResponseDto>(true, Data, 200);
            
        }
        public async Task<ApiResponseDto<CategoryResponseDto>> UpdateCategory(UpdateCategoryDto dto)
        {
            var category = await _categoryRepo.GetByIdAsync(dto.Id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {dto.Id} not found.");
            }
            category.CategoryName = dto.Name;
            category.Description = dto.decription;
            var Data = new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
            };
            await _categoryRepo.UpdateAsync(category);
            return new ApiResponseDto<CategoryResponseDto>(true, Data, 200);

        }
        public async Task<ApiResponseDto<CategoryResponseDto>> deteletCategory(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with ID {id} not found.");
            }
            await _categoryRepo.DeleteAsync(category);
            var Data = new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description,
            };
            return new ApiResponseDto<CategoryResponseDto>(true, Data, 200);
        }
        
    }
}
