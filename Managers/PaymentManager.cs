using Interfaces.IRepository;
using Interfaces.IManager;
using Stripe;
using Enity=DataAccess.Entities;
using Microsoft.Extensions.Configuration;
using Models.DTOs;
namespace Managers
{
    public class PaymentManager: IPaymentManager
    {
        private readonly IRepository<Enity.Order> _orderRepository;
        private readonly IRepository<Enity.Payment> _paymentRepository;
        private readonly IConfiguration _configuration;
        
        public PaymentManager(IRepository<Enity.Order> orderRepository,
                              IRepository<Enity.Payment> paymentRepository,
                              IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _configuration = configuration;
            var secretKey=_configuration["StripeSettings:SecretKey"];
            StripeConfiguration.ApiKey=secretKey;
        }
        public async Task<(string PaymentIntentId,string ClientSecret)> CreatePaymentIntentAsync(int orderId, decimal amount)
        {
            var currency=_configuration["StripeSettings:Currency"] ?? "inr";
            var createOptions=new PaymentIntentCreateOptions{
                Amount=(long)Math.Round(amount*100M,MidpointRounding.ToEven),
                Currency=currency,
                PaymentMethodTypes=new List<string>{"card"},
                Metadata=new Dictionary<string, string>
                {
                    {"order_id",orderId.ToString()}
                }
            };
            var service=new PaymentIntentService();
            var intent=await service.CreateAsync(createOptions);
            var payment=new Enity.Payment
            {
                OrderId=orderId,
                PaymentIntentId=intent.Id,
                Amount=amount,
                PaymentDate=DateTime.Now,
                Status="Pending",
            };
            await _paymentRepository.AddAsync(payment);
            return (intent.Id, intent.ClientSecret);
        }
        public async Task<Result> ConfirmPaymentAsync(int orderId, string paymentId)
        {
            var order=await _orderRepository.GetByIdAsync(orderId);
            if(order==null)
            {
                return new Result{Success=false,Message="Order not found"};
            }
            var confirmOptions=new PaymentIntentConfirmOptions
            {
                PaymentMethod="pm_card_visa"
            };
            var service=new PaymentIntentService();
            var intent=await service.ConfirmAsync(paymentId,confirmOptions);
            if(!string.Equals(intent.Status,"succeeded",StringComparison.OrdinalIgnoreCase)){
                return new Result{Success=false,Message="Payment confirmation failed"};
            }
            var payment=(await _paymentRepository.GetAllAsync()).FirstOrDefault(p=>p.PaymentIntentId==paymentId);
            if (payment == null)
            {
                var newPayment=new Enity.Payment
                {
                    OrderId=orderId,
                    PaymentIntentId=paymentId,
                    Amount=order.TotalAmount,
                    PaymentDate=DateTime.Now,
                    Status="Succeeded"
                };
                await _paymentRepository.AddAsync(newPayment);
            }
            else
            {
                payment.Status="Succeeded";
                await _paymentRepository.UpdateAsync(payment);
            }
            order.PaymentStatus="paid";
            await _orderRepository.UpdateAsync(order);
        return new Result{Success=true,Message="Payment confirmed and order updated successfully"};
        }
        public async Task<Result> HandlePaymentWebhookAsync(string json, string sigHeader)
        {
            var webhook=_configuration["StripeSettings:WebhookSecret"];
            Event stripeEvent;
            try
            {
                stripeEvent=EventUtility.ConstructEvent(json,sigHeader,webhook!);
            }catch(Exception ex)
            {
                return new Result{Success=false,Message=$"Webhook error: {ex.Message}"};
            }
            if(stripeEvent.Type=="payment_intent.succeeded"){
                var paymentIntent=stripeEvent.Data.Object as PaymentIntent;
                return new Result{Success=true,Message="Payment intent succeeded webhook handled",Data=paymentIntent};
            }
            if (stripeEvent.Type == "payment_intent.payment_failed")
            {
                var paymentIntent=stripeEvent.Data.Object as PaymentIntent;
                return new Result{Success=true,Message="Payment intent failed webhook handled",Data=paymentIntent};
            }
            return new Result{Success=false,Message="Unhandled event type",Data=stripeEvent.Type};
        }
    }
}