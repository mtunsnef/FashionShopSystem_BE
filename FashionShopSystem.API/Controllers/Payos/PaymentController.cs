using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using FashionShopSystem.Service.DTOs.PayOsDto;

namespace FashionShopSystem.API.Controllers.Payos
{
    [Route("[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly PayOS _payOS;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(PayOS payOS, ILogger<PaymentController> logger)
        {
            _payOS = payOS;
            _logger = logger;
        }


        [HttpPost("payos_transfer_handler")]
        public IActionResult payOSTransferHandler(WebhookType body)
        {
            try
            {
                WebhookData data = _payOS.verifyPaymentWebhookData(body);

                if (data.description == "Ma giao dich thu nghiem" || data.description == "VQRIO123")
                {
                    return Ok(new Response(0, "Ok", null));
                }
                return Ok(new Response(0, "Ok", null));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return Ok(new Response(-1, "fail", null));
            }

        }

    }
}

