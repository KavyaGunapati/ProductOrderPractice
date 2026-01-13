using Models.DTOs;

namespace Interfaces.IManager
{
    public interface IPaymentManager
    {
        Task<(string PaymentIntentId,string ClientSecret)> CreatePaymentIntentAsync(int orderId, decimal amount);
        Task<Result> ConfirmPaymentAsync(int orderId, string paymentId);
        Task<Result> HandlePaymentWebhookAsync(string payload, string signature);
    }
}