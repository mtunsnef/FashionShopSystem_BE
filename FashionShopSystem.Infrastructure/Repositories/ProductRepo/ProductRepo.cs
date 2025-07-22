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
    }
}
