using Entity=DataAccess.Entities;
using Interfaces.IRepository;
using Models.DTOs;
using AutoMapper;
using Interfaces.IManager;
namespace Managers
{
    public class ProductManager: IProductManager
    {
        private readonly IRepository<Entity.Product> _productRepository;
        private readonly IMapper _mapper;
        public ProductManager(IRepository<Entity.Product> productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }
        public async Task<Result<IEnumerable<Product>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _productRepository.GetAllAsync();
                return new Result<IEnumerable<Product>>
                {
                    Success = true,
                    Data = _mapper.Map<IEnumerable<Product>>(products)
                };
            }
            catch (Exception ex)
            {
                return new Result<IEnumerable<Product>>
                {
                    Success = false,
                    Message = $"An error occurred while retrieving products: {ex.Message}"
                };
            }
        }
        public async Task<Result<Product?>> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return new Result<Product?>
                {
                    Success = false,
                    Data = null,
                    Message = "Product not found."
                };
            }
            return new Result<Product?>
            {
                Success = product != null,
                Data = _mapper.Map<Product?>(product),
                Message =" Product retrieved successfully."
            };
        }
        public async Task<Result> AddProductAsync(Product product)
        {
            try
            {
                var entity=_mapper.Map<Entity.Product>(product);
                await _productRepository.AddAsync(entity);
                return new Result
                {
                    Success = true,
                    Message = "Product added successfully."
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"An error occurred while adding the product: {ex.Message}"
                };
            }
        }
        public async Task<Result<Product>> UpdateProductAsync(Product product)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(product.Id);
                if (existingProduct == null)
                {
                    throw new Exception("Product not found.");
                }
                _mapper.Map(product,existingProduct);
                await _productRepository.UpdateAsync(existingProduct);
                return new Result<Product>
                {
                    Success = true,
                    Data = _mapper.Map<Product>(existingProduct),
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new Result<Product>
                {
                    Success = false,
                    Message = $"An error occurred while updating the product: {ex.Message}"
                };
            }
        }
        public async Task<Result> DeleteProductAsync(int productId)
        {
            try
            {
                var existingProduct = await _productRepository.GetByIdAsync(productId);
                if (existingProduct == null)
                {
                   return new Result
                    {
                        Success = false,
                        Message = "Product not found."
                    };
                }
                await _productRepository.DeleteAsync(existingProduct);
                return new Result
                {
                    Success = true,
                    Message = "Product deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    Success = false,
                    Message = $"An error occurred while deleting the product: {ex.Message}"
                };
            }
        }
    }
}