using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.DTOs.PayOsDto
{
    public class PayOSResponseModel
    {
        public int Amount { get; set; }
        public bool IsSuccess { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Thông tin cố định
        public string ItemName => "Thanh toán";
        public string PaymentMethod => "PayOS";
        public string Status => IsSuccess ? "Thành công" : "Đã hủy";
    }
}
