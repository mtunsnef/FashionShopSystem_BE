using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using Net.payOS;
using FashionShopSystem.Domain.Exceptions.Http;

namespace FashionShopSystem.API.Controllers.Payos
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutController : Controller
    {
        private readonly PayOS _payOS;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CheckoutController> _logger;

        public CheckoutController(
            PayOS payOS,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CheckoutController> logger)
        {
            _payOS = payOS;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            // Trả về trang HTML có tên "MyView.cshtml"
            return View("index");
        }


        [HttpPost("create-payment-link")]
        public async Task<IActionResult> CheckoutPayment([FromForm] int postId, [FromForm] int AmountPayOs)
        {
            try
            {
                int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

                ItemData item = new ItemData("Đơn hàng mua sản phẩm", 1, AmountPayOs);
                List<ItemData> items = new List<ItemData> { item };

                var request = _httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";

                if (AmountPayOs <= 0)
                {
                    _logger.LogWarning("AmountPayOs <= 0 for PostId: {PostId}", postId);
                    throw new BadRequestException("Số tiền không hợp lệ.");
                }

                PaymentData paymentData = new PaymentData(
                    orderCode,
                    AmountPayOs,
                    "Thanh toán đơn hàng",
                    items,
                    $"{baseUrl}/cancel?postId={postId}&amount={AmountPayOs}",
                    $"{baseUrl}/success?postId={postId}&amount={AmountPayOs}"
                );

                CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
                _logger.LogInformation("Created payment link for PostId: {PostId}, OrderCode: {OrderCode}",
                    postId, orderCode);

                return Ok(new { url = createPayment.checkoutUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment link for PostId: {PostId}", postId);
                throw new BadRequestException("Đã xảy ra lỗi khi tạo liên kết thanh toán.", ex);
            }
        }
    }
}
