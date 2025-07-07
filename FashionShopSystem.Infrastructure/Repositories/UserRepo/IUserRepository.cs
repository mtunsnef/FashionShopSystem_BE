using FashionShopSystem.Domain.Models;

namespace FashionShopSystem.Infrastructure.Repositories.UserRepo
{
	public interface IUserRepository : IRepositoryBase<User>
	{
		Task<User?> GetAccountByEmail(string email);
	}
}
