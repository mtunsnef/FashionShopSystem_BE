using FashionShopSystem.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Domain.Utils
{
    public static class PaymentStatusHelper
    {
        public static string ToDbValue(this PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "Unpaid",
                PaymentStatus.Paid => "Paid",
                PaymentStatus.Cancelled => "Cancelled",
                PaymentStatus.Failed => "Failed",
                PaymentStatus.Refunded => "Refunded",
                _ => throw new ArgumentOutOfRangeException(nameof(status), $"Unsupported status: {status}")
            };
        }

        public static PaymentStatus FromDbValue(string value)
        {
            return value switch
            {
                "Unpaid" => PaymentStatus.Unpaid,
                "Paid" => PaymentStatus.Paid,
                "Cancelled" => PaymentStatus.Cancelled,
                "Failed" => PaymentStatus.Failed,
                "Refunded" => PaymentStatus.Refunded,
                _ => throw new ArgumentOutOfRangeException(nameof(value), $"Unsupported status value: {value}")
            };
        }

        public static string ToDisplay(this PaymentStatus status)
        {
            return status switch
            {
                PaymentStatus.Unpaid => "CẦN THANH TOÁN",
                PaymentStatus.Paid => "ĐÃ THANH TOÁN",
                PaymentStatus.Cancelled => "ĐÃ HỦY",
                PaymentStatus.Failed => "THẤT BẠI",
                PaymentStatus.Refunded => "ĐÃ HOÀN TIỀN",
                _ => string.Empty
            };
        }
    }
}
