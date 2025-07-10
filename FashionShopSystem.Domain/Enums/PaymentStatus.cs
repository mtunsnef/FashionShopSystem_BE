using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Domain.Enums
{
    public enum PaymentStatus
    {
        Unpaid,     // Chờ thanh toán
        Paid,       // Đã thanh toán
        Cancelled,  // Hủy
        Failed,     // Thất bại
        Refunded    // Hoàn tiền
    }
}
