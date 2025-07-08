using FashionShopSystem.Domain.Models;
using FashionShopSystem.Service.DTOs.ApiResponseDto;
using FashionShopSystem.Service.DTOs;


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
        Task<IEnumerable<ProductDto>> SearchProductsAsync(int? categoryId, string? brand, decimal? minPrice, decimal? maxPrice, string? keyword);
    }
}
