using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.DTOs.OrderDto
{
    public class CreateOrderFromCheckoutDto
    {
        public required string FullName { get; set; }
        public required string Phone { get; set; }
        public required string Email { get; set; }
        public required string ShippingAddress { get; set; }
        public string? Note { get; set; }
        public List<OrderItemFromCheckoutDto> CartItems { get; set; } = new();
    }

    public class OrderItemFromCheckoutDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
