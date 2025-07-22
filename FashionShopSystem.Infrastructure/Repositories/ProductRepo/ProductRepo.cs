using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FashionShopSystem.Infrastructure
{
    public class ProductRepo : RepositoryBase<Product>, IProductRepo
    {
        public ProductRepo(FashionShopContext context) : base(context)
        {
        }
        public override async Task<List<Product>> GetAllAsync()
        {
            var products =  _dbSet
                .Include(p => p.Category)
                .Include(p => p.Favorites)
                .Include(p => p.OrderDetails)
                .AsQueryable();
            return await products.ToListAsync();
        }
        public override async Task<Product?> GetByIdAsync(object id)
        {
            return await _dbSet
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == (int)id);
        }

        public async Task<IEnumerable<Product>> GetFilteredProductsAsync(int? categoryId, string? brand, decimal? minPrice, decimal? maxPrice, string? keyword)
        {
            var query = _dbSet.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand!.ToLower().Contains(brand.ToLower()));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string lowerKeyword = keyword.ToLower();
                query = query.Where(p =>
                    p.ProductName!.ToLower().Contains(lowerKeyword) ||
                    p.Description!.ToLower().Contains(lowerKeyword) ||
                    p.Brand!.ToLower().Contains(lowerKeyword) ||
                    (p.Category != null && p.Category.CategoryName!.ToLower().Contains(lowerKeyword))
                );
            }

            return await query.ToListAsync();
        }
    }
}
