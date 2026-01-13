using System.Reflection.Metadata.Ecma335;
using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;

namespace OrderPRoductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductManager _productManager;
        public ProductController(IProductManager productManager)
        {
            _productManager = productManager;
        }
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            var result=await _productManager.AddProductAsync(product);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result=await _productManager.GetAllProductsAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var result=await _productManager.GetProductByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateProduct([FromBody] Product product)
        {
            var result=await _productManager.UpdateProductAsync(product);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
     }
}