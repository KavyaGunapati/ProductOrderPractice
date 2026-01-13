using Interfaces.IRepository;
using Models.DTOs;
using AutoMapper;
using Interfaces.IManager;
using Entity=DataAccess.Entities;
namespace Managers
{
    public class OrderManager: IOrderManager
    {
        private readonly IRepository<Entity.Order> _orderRepository;
        private readonly IMapper _mapper;
        private readonly IRepository<Entity.Product> _productRepository;
        private readonly IRepository<Entity.OrderItem> _orderItemRepository;
        public OrderManager(IRepository<Entity.Order> orderRepository, IMapper mapper, IRepository<Entity.Product> productRepository, IRepository<Entity.OrderItem> orderItemRepository)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _productRepository = productRepository;
            _orderItemRepository = orderItemRepository;
        }
        public async Task<Result<IEnumerable<Order>>> GetAllOrdersAsync()
        {
            var entities=await _orderRepository.GetAllAsync();
            var dtos=_mapper.Map<IEnumerable<Order>>(entities);
            return new Result<IEnumerable<Order>>{Success=true,Message="Orders retrieved successfully",Data=dtos};
        }
        public async Task<Result<Order>> GetOrderByIdAsync(int id)
        {
            var entity=await _orderRepository.GetByIdAsync(id);
            if(entity==null)
            {
                return new Result<Order>{Success=false,Message="Order not found"};
            }
            var dto=_mapper.Map<Order>(entity);
            return new Result<Order>{Success=true,Message="Order retrieved successfully",Data=dto};
        }
        public async Task<Result> AddOrderAsync(Order order,string appUserId)
        {
            var entity=new Entity.Order
            {
                OrderDate=DateTime.Now,
                OrderItems=new List<Entity.OrderItem>(),
                TotalAmount=0,
                AppUserId=appUserId,
                PaymentStatus="Pending"
            };
            
            decimal totalAmount=0;
            foreach(var item in order.OrderItems)
            {
                var product=await _productRepository.GetByIdAsync(item.ProductId);
                if(product!=null)
                {
                    var orderItemEntity=new Entity.OrderItem
                    {
                        ProductId=item.ProductId,
                        Quantity=item.Quantity
                    };
                    entity.OrderItems.Add(orderItemEntity);
                    product.Stock-=item.Quantity;
                    await _productRepository.UpdateAsync(product);
                    totalAmount+=item.Quantity * product.Price;
                }
            }
            
            entity.TotalAmount = totalAmount;
            await _orderRepository.AddAsync(entity);
            return new Result{Success=true,Message="Order added successfully"};
        }
        public async Task<Result> UpdateOrderAsync(Order order)
        {
            var existingEntity=await _orderRepository.GetByIdAsync(order.Id);
            if(existingEntity==null)
            {
                return new Result{Success=false,Message="Order not found"};
            }
            _mapper.Map(order, existingEntity);
            await _orderRepository.UpdateAsync(existingEntity);
            return new Result{Success=true,Message="Order updated successfully"};
        }
        public async Task<Result> DeleteOrderAsync(int id)
        {
            var existingEntity=await _orderRepository.GetByIdAsync(id);
            if(existingEntity==null)
            {
                return new Result{Success=false,Message="Order not found"};
            }
            await _orderRepository.DeleteAsync(existingEntity);
            return new Result{Success=true,Message="Order deleted successfully"};
        }
    }
}