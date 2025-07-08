using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;

namespace FashionShopSystem.Infrastructure
{
    public interface IProductRepo : IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> GetFilteredProductsAsync(int? categoryId, string? brand, decimal? minPrice, decimal? maxPrice, string? keyword);
    }
}
