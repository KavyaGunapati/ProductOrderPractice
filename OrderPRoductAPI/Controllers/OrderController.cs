using System.Security.Claims;
using Interfaces.IManager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace OrderPRoductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderManager _orderManager;
        public OrderController(IOrderManager orderManager)
        {
            _orderManager = orderManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var result=await _orderManager.GetAllOrdersAsync();
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result=await _orderManager.GetOrderByIdAsync(id);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            var userId=User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result=await _orderManager.AddOrderAsync(order,userId!);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateOrder([FromBody] Order order)
        {
            var result=await _orderManager.UpdateOrderAsync(order);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result=await _orderManager.DeleteOrderAsync(id);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}