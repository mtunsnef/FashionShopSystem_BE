
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using Microsoft.AspNetCore.Http;

namespace FashionShopSystem.Service
{
    public interface IProductService 
    {
        public Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync(int? categoryid, string? keyword,string? sort, string? brand, decimal? price);
        public Task<ProductResponseDto> GetProductByIdAsync(int id);
        public Task<ApiResponseDto<ProductResponseDto>> UpdateProduct(UpdateProductDto dto);
        public Task<ApiResponseDto<Product>> CreateProduct(CreateProductDto dto);
        public Task<ApiResponseDto<ProductResponseDto>> deteletProduct(int id);
        public Task<List<string>> getAllBrand();
    }
}
