using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;

namespace OrderPRoductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentManager _paymentManager;
        public PaymentController(IPaymentManager paymentManager)
        {
            _paymentManager = paymentManager;
        }
        [HttpPost("create-payment-intent")]
        public async Task<IActionResult> CreatePaymentIntent(int OrderId, decimal amount)
        {
            var (paymentIntentId, clientSecret) = await _paymentManager.CreatePaymentIntentAsync(OrderId, amount);
            return Ok(new
            {
                success = true,
                message = "Payment intent created successfully",
                data = new
                {
                    paymentIntentId,
                    clientSecret
                }
            });
        }
        [HttpPost("confirm-payment")]
        public async Task<IActionResult> ConfirmPayment([FromQuery] int orderId,string paymentId)
        {
            var result=await _paymentManager.ConfirmPaymentAsync(orderId,paymentId);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            try
            {
                var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
                var stripeSignature = HttpContext.Request.Headers["Stripe-Signature"].ToString();
                
                // Validate webhook data
                if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(stripeSignature))
                {
                    return BadRequest(new { success = false, message = "Missing webhook data or signature" });
                }
                
                var result = await _paymentManager.HandlePaymentWebhookAsync(json, stripeSignature);
                if (result.Success)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = $"Webhook error: {ex.Message}" });
            }
        }
    }
}