using EvilCorp2000.Entities;

namespace EvilCorp2000.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Guid productId);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductById(Guid id);
        Task UpdateProduct (Product productToStore, Product productFromDB);
        Task<bool> IsProductNameUniqueAsync(string name);
    }
}