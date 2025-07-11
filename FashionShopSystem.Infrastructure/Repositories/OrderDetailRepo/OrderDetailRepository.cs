using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories.OrderRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Infrastructure.Repositories.OrderDetailRepo
{
    public class OrderDetailRepository : RepositoryBase<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(FashionShopContext context) : base(context)
        {
        }
    }
}