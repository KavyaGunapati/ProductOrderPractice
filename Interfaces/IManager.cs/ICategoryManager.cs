using Models.DTOs;

namespace Interfaces.IManager
{
    public interface ICategoryManager
    {
        Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync();
        Task<Result<Category>> GetCategoryByIdAsync(int id);
        Task<Result> AddCategoryAsync(Category category);
        Task<Result> UpdateCategoryAsync(Category category);
        Task<Result> DeleteCategoryAsync(int id);
    }
}