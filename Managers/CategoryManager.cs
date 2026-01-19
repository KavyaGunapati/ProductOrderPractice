using Entity=DataAccess.Entities;
using Interfaces.IRepository;
using Models.DTOs;
using AutoMapper;
using Interfaces.IManager;

namespace Managers
{
    public class CategoryManager: ICategoryManager
    {
        private readonly IRepository<Entity.Category> _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryManager(IRepository<Entity.Category> categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<Category>>> GetAllCategoriesAsync()
        {
            var entities=await _categoryRepository.GetAllAsync();
            var dtos=_mapper.Map<IEnumerable<Category>>(entities);
            return new Result<IEnumerable<Category>>{Success=true,Message="Categories retrieved successfully",Data=dtos};
        }
        public async Task<Result<Category>> GetCategoryByIdAsync(int id)
        {
            var entity=await _categoryRepository.GetByIdAsync(id);
            if(entity==null)
            {
                return new Result<Category>{Success=false,Message="Category not found"};
            }
            var dto=_mapper.Map<Category>(entity);
            return new Result<Category>{Success=true,Message="Category retrieved successfully",Data=dto};
        }
        public async Task<Result> AddCategoryAsync(Category category)
        {
            try
            {
                var entity=_mapper.Map<Entity.Category>(category);
                await _categoryRepository.AddAsync(entity);
                return new Result{Success=true,Message="Category added successfully"};
            }
            catch(Exception ex)
            {
                return new Result{Success=false,Message=$"An error occurred while adding the category: {ex.Message}"};
            }
        }
        public async Task<Result> UpdateCategoryAsync(Category category)
        {
            try
            {
                var existingEntity=await _categoryRepository.GetByIdAsync(category.Id);
            if(existingEntity==null)
            {
                return new Result{Success=false,Message="Category not found"};
            }
            var entity=_mapper.Map<Entity.Category>(category);
            await _categoryRepository.UpdateAsync(entity);
            return new Result{Success=true,Message="Category updated successfully"};
            }catch(Exception ex)
            {
                return new Result{Success=false,Message=$"An error occurred while updating the category: {ex.Message}"};
            }
        }
        public async Task<Result> DeleteCategoryAsync(int id)
        {
            try
            {
                var existingEntity=await _categoryRepository.GetByIdAsync(id);
            if(existingEntity==null)
            {
                return new Result{Success=false,Message="Category not found"};
            }
            await _categoryRepository.DeleteAsync(existingEntity);
            return new Result{Success=true,Message="Category deleted successfully"};
            }
            catch(Exception ex)
            {
                return new Result{Success=false,Message=$"An error occurred while deleting the category: {ex.Message}"};
            }
        }
    }
}