using DataAccess.Entities;
using Interfaces.IRepository;

namespace Managers
{
    public class CategoryManager: ICategoryManager
    {
        private readonly IRepository<Category> _categoryRepository;
        public CategoryManager(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
    }
}