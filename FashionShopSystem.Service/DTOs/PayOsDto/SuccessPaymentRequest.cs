using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.DTOs.PayOsDto
{
    public class SuccessPaymentRequest
    {
        public int OrderId { get; set; }
        public int Amount { get; set; }
        public string Code { get; set; }
        public string Id { get; set; }
        public bool Cancel { get; set; }
        public string Status => "Thành công";
        public int OrderCode { get; set; }
    }
}
