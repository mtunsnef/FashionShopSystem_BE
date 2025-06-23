using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
