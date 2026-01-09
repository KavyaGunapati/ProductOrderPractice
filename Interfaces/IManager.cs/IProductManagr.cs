using Models.DTOs;

namespace Interfaces.IManager
{
    public interface IProductManager
    {
        Task<Result<IEnumerable<Product>>> GetAllProductsAsync();
        Task<Result<Product?>> GetProductByIdAsync(int productId);
        Task<Result> AddProductAsync(Product product);
        Task<Result<Product>> UpdateProductAsync(Product product);
        Task<Result> DeleteProductAsync(int productId);
    }
}