using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure;

namespace FashionShopSystem.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepo _productRepo;
        public ProductService(IProductRepo productRepo)
        {
            _productRepo = productRepo;
        }
        public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
        {

            var product = await _productRepo.GetAllAsync();
            var listProduct = product.Select(p => new ProductResponseDto
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                CategoryId = p.CategoryId,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
            });
            return listProduct;
        }
    }
}
