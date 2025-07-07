using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FashionShopSystem.Service
{
    public class UpdateProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }

        public string? Description { get; set; }

        public decimal? Price { get; set; }

        public int? Stock { get; set; }

        public int? CategoryId { get; set; }

        public string? Brand { get; set; }

        public IFormFile? ImageUrl { get; set; }
        public bool? IsActive { get; set; }
    }
}
