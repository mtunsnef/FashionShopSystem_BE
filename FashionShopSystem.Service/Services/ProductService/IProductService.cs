
using FashionShopSystem.Infrastructure;

namespace FashionShopSystem.Service
{
    public interface IProductService 
    {
        public Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    }
}
