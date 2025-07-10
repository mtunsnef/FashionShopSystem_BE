using FashionShopSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Domain.Utils
{
    public static class DeliveryStatusHelper
    {
        public static string ToDbValue(this DeliveryStatus status)
        {
            return status switch
            {
                DeliveryStatus.Processing => "Processing",
                DeliveryStatus.Shipped => "Shipped",
                DeliveryStatus.Delivered => "Delivered",
                DeliveryStatus.Cancelled => "Cancelled",
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unsupported status: {status}")
            };
        }

        public static DeliveryStatus FromDbValue(string value)
        {
            return value switch
            {
                "Processing" => DeliveryStatus.Processing,
                "Shipped" => DeliveryStatus.Shipped,
                "Delivered" => DeliveryStatus.Delivered,
                "Cancelled" => DeliveryStatus.Cancelled,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported status value: {value}")
            };
        }

        public static string ToDisplay(this DeliveryStatus status)
        {
            return status switch
            {
                DeliveryStatus.Processing => "ĐANG XỬ LÝ",
                DeliveryStatus.Shipped => "ĐÃ GIAO HÀNG",
                DeliveryStatus.Delivered => "ĐÃ NHẬN HÀNG",
                DeliveryStatus.Cancelled => "ĐÃ HỦY",
                _ => string.Empty
            };
        }
    }
}
