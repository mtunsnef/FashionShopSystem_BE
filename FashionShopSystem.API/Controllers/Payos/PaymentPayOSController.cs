using FashionShopSystem.Domain.Enums;
using FashionShopSystem.Domain.Exceptions.Http;
using FashionShopSystem.Domain.Utils;
using FashionShopSystem.Infrastructure;
using FashionShopSystem.Infrastructure.Repositories.OrderRepo;
using FashionShopSystem.Service.DTOs.PayOsDto;
using FashionShopSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace FashionShopSystem.API.Controllers.Payos
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentPayOSController : ControllerBase
    {
        private readonly ILogger<PaymentPayOSController> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepo _productRepository;
        private readonly IConfiguration _configuration;

        public PaymentPayOSController(
            ILogger<PaymentPayOSController> logger,
            IOrderRepository orderRepository,
            IProductRepo productRepository, IConfiguration configuration)
        {
            _logger = logger;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Xử lý thanh toán thành công
        /// </summary>
        [HttpGet("success")]
        public async Task<IActionResult> Success([FromQuery] SuccessPaymentRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

            order.PaymentStatus = PaymentStatus.Paid.ToDbValue();
            order.DeliveryStatus = DeliveryStatus.Processing.ToDbValue();
            await _orderRepository.UpdateAsync(order);

            var mailService = new MailService(_configuration);
            var subject = "ShopClothes - Thanh toán thành công";
            var content = $@"
                <h2>Chào {order.FullName},</h2>
                <p>Bạn đã thanh toán thành công đơn hàng #{request.Id} với số tiền <strong>{request.Amount:N0} ₫</strong>.</p>
                <p>Chúng tôi sẽ giao hàng cho bạn trong thời gian sớm nhất.</p>
                <p>Cảm ơn bạn đã mua sắm tại <strong>ShopClothes</strong>.</p>
            ";
            await mailService.SendEmailAsync(order.Email!, subject, content);

            var feRedirectUrl = $"https://localhost:7298/success?" +
                                $"orderId={request.OrderId}&amount={request.Amount}&code={request.Code}" +
                                $"&id={request.Id}&status={HttpUtility.UrlEncode(request.Status)}&orderCode={request.OrderCode}";

            return Redirect(feRedirectUrl);
        }

        /// <summary>
        /// Xử lý khi hủy thanh toán
        /// </summary>
        [HttpGet("cancel")]
        public async Task<IActionResult> Cancel([FromQuery] CancelPaymentRequest request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId)
                ?? throw new NotFoundException("Không tìm thấy đơn hàng");

            order.PaymentStatus = PaymentStatus.Cancelled.ToDbValue();
            order.DeliveryStatus = DeliveryStatus.Cancelled.ToDbValue();
            await _orderRepository.UpdateAsync(order);

            foreach (var detail in order.OrderDetails)
            {
                var product = await _productRepository.GetByIdAsync(detail.ProductId);
                if (product is not null)
                {
                    product.Stock += detail.Quantity;
                    await _productRepository.UpdateAsync(product);
                }
            }

            var mailService = new MailService(_configuration);
            var subject = "ShopClothes - Huỷ thanh toán đơn hàng";
            var content = $@"
                <h2>Xin chào {order.FullName},</h2>
                <p>Đơn hàng #{request.Id} của bạn đã được <strong>hủy thanh toán</strong>.</p>
                <p>Nếu đây là nhầm lẫn, bạn có thể đặt lại đơn hàng trên website.</p>
                <p>Cảm ơn bạn đã ghé thăm <strong>ShopClothes</strong>.</p>
            ";
            await mailService.SendEmailAsync(order.Email!, subject, content);

            var statusEncoded = HttpUtility.UrlEncode(request.Status);

            var feRedirectUrl = $"https://localhost:7298/cancel?" +
                                $"orderId={request.OrderId}&amount={request.Amount}&code={request.Code}" +
                                $"&id={request.Id}&cancel={request.Cancel}&status={statusEncoded}&orderCode={request.OrderCode}";

            return Redirect(feRedirectUrl);
        }
    }
}