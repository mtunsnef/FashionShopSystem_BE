using FashionShopSystem.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FashionShopSystem.Infrastructure.Repositories.UserRepo
{
	public class UserRepository : RepositoryBase<User>, IUserRepository
	{
		public UserRepository(FashionShopContext context) : base(context)
		{
		}

		public async Task<User?> GetAccountByEmail(string email) => await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
	}
}
