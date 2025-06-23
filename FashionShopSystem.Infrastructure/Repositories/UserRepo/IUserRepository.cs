using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Infrastructure.Repositories.UserRepo
{
    public interface IUserRepository : IRepositoryBase<User>
    {
        Task<User?> GetAccountByEmail(string email);
    }
}
