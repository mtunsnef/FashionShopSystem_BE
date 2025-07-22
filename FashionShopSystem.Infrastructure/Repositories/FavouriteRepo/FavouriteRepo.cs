
using Microsoft.EntityFrameworkCore;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;

namespace FashionShopSystem.Infrastructure
{
    public class FavouriteRepo : RepositoryBase<Favorite>, IFavouriteRepo
    {
        public FavouriteRepo(FashionShopContext context) : base(context)
        {
        }
        public async Task<List<Favorite>> GetFavoritesByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(f=>f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }
    }
}
