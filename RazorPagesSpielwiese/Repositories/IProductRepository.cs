using EvilCorp2000.Entities;

namespace EvilCorp2000.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductById(Guid id);
        Task UpdateProduct(Product product);
        Task<bool> IsProductNameUniqueAsync(string name);
    }
}