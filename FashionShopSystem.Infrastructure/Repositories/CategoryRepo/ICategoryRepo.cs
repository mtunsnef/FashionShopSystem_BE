﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FashionShopSystem.Domain.Models;
using FashionShopSystem.Infrastructure.Repositories;

namespace FashionShopSystem.Infrastructure
{
    public interface ICategoryRepo : IRepositoryBase<Category>
    {
    }
}
