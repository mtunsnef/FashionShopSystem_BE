using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FashionShopSystem.Service.DTOs.AuthDto
{
    public class TwoFaVerifyDto
    {
        public string Code { get; set; }
        public string SharedKey { get; set; }
    }
}
