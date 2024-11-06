using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Product product);
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetProductById(Guid id);
        Task UpdateProduct(Product product);
        Task<List<Category>> GetCategoriesForProduct(Product product);
    }
}