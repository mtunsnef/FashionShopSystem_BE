//using FashionShopSystem.Service.DTOs.PayOsDto;
//using FashionShopSystem.Service.Services.OrderService;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace FashionShopSystem.API.Controllers.Payos
//{
//    [Route("[controller]")]
//    public class PaymentPayOSController : Controller
//    {
//        private readonly ILogger<PaymentPayOSController> _logger;
//        private readonly IOrderService _orderService;
//        public PaymentPayOSController(ILogger<PaymentPayOSController> logger, IOrderService orderService)
//        {
//            _logger = logger;
//            _orderService = orderService;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View("Error!");
//        }

//        [HttpGet("success")]
//        public async Task<IActionResult> Success([FromQuery] int postId, [FromQuery] int amount)
//        {
//            try
//            {
//                var post = await _postService.GetByIdAsync(postId);
//                if (post == null)
//                    return NotFound("Post not found");

//                var postPackage = await _postDetailPackageService.GetByPostIdAsync(postId);
//                if (postPackage == null)
//                    return NotFound("Post package not found");

//                string packageTypeName = postPackage.Pricing?.PackageType.PackageTypeName ?? "Tin thường";
//                var packageType = BadgeHelper.ParsePackageType(packageTypeName);

//                if (BadgeHelper.IsVip(packageType))
//                {
//                    post.CurrentStatus = PostStatusHelper.ToDbValue(PostStatus.Active);
//                }
//                else
//                {
//                    post.CurrentStatus = PostStatusHelper.ToDbValue(PostStatus.Pending);
//                }

//                postPackage.PaymentStatus = PostPackagePaymentStatusHelper.ToDbValue(PostPackagePaymentStatus.Completed);
//                //postPackage.PaymentTransactionId = transactionId;

//                var payment = new Payment
//                {
//                    PostPackageDetailsId = postPackage.Id,
//                    TotalPrice = postPackage.TotalPrice,
//                    Status = PaymentStatusHelper.ToDbValue(PaymentStatus.Success),
//                    PaymentDate = DateTime.Now,
//                    CreatedAt = DateTime.Now,
//                    UpdatedAt = DateTime.Now,
//                    MethodId = 2,
//                    AccountId = post.AccountId
//                };

//                await _paymentService.AddAsync(payment);
//                await _postService.UpdateAsync(post);
//                await _postDetailPackageService.UpdateAsync(postPackage);

//                var model = new PayOSResponseModel
//                {
//                    Amount = amount,
//                    IsSuccess = true,
//                    CreatedAt = DateTime.Now
//                };

//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Payment success processing failed for postId {PostId}", postId);
//                return Redirect("/?error=payment_error");
//            }
//        }

//        [HttpGet("/cancel")]
//        public async Task<IActionResult> Cancel([FromQuery] int postId, [FromQuery] int amount = 0)
//        {
//            try
//            {
//                if (postId <= 0)
//                {
//                    _logger.LogWarning("Cancel payment called without valid postId");
//                    return BadRequest("Invalid postId");
//                }

//                var post = await _postService.GetByIdAsync(postId);
//                if (post == null)
//                {
//                    _logger.LogWarning("Post not found for cancellation. postId={PostId}", postId);
//                    return NotFound("Post not found");
//                }

//                var postPackage = await _postDetailPackageService.GetByPostIdAsync(postId);
//                if (postPackage == null)
//                {
//                    _logger.LogWarning("PostPackageDetail not found for cancellation. postId={PostId}", postId);
//                    return NotFound("Post package not found");
//                }

//                post.CurrentStatus = PostStatusHelper.ToDbValue(PostStatus.Cancelled);
//                postPackage.PaymentStatus = PostPackagePaymentStatusHelper.ToDbValue(PostPackagePaymentStatus.Inactive);

//                await _postService.UpdateAsync(post);
//                await _postDetailPackageService.UpdateAsync(postPackage);

//                var model = new PayOSResponseModel
//                {
//                    Amount = amount > 0 ? amount : 50000,
//                    IsSuccess = false,
//                    CreatedAt = DateTime.Now
//                };

//                _logger.LogInformation("Payment cancelled for postId={PostId}, amount={Amount}", postId, amount);
//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error processing cancelled payment");
//                return RedirectToAction("Error", "Home");
//            }
//        }
//    }
//}
