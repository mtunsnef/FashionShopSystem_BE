﻿

namespace FashionShopSystem.Service.DTOs
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Brand { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; } 
    }

}
