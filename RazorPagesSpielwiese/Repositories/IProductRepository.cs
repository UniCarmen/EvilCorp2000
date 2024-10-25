using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductById(int id);
        Task UpdateProduct(Product product);
    }
}