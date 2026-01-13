using Models.DTOs;

namespace Interfaces.IManager
{
    public interface IOrderManager
    {
        Task<Result<IEnumerable<Order>>> GetAllOrdersAsync();
        Task<Result<Order>> GetOrderByIdAsync(int id);
        Task<Result> AddOrderAsync(Order order, string appUserId);
        Task<Result> UpdateOrderAsync(Order order);
        Task<Result> DeleteOrderAsync(int id);
    }
}