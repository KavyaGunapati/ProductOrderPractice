using Interfaces.IManager;
using Microsoft.AspNetCore.Mvc;
using Models.DTOs;
namespace OrderPRoductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryManager _categoryManager;
        public CategoryController(ICategoryManager categoryManager)
        {
            _categoryManager = categoryManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryManager.GetAllCategoriesAsync();
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var result = await _categoryManager.GetCategoryByIdAsync(id);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Category category)
        {
            var result = await _categoryManager.AddCategoryAsync(category);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpPatch]
        public async Task<IActionResult> UpdateCategory([FromBody] Category category)
        {
            var result = await _categoryManager.UpdateCategoryAsync(category);
            if (result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result=await _categoryManager.DeleteCategoryAsync(id);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}