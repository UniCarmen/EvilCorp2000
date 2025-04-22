using DataAccess.Entities;
using static Shared.Utilities.Utilities;

namespace DataAccess.Repositories
{
    public interface IProductRepository
    {
        Task AddProduct(Product product);
        Task DeleteProduct(Guid productId);
        Task<List<Product>> GetAllProductsAsync(ProductSortOrder? sortOrder = null, int? pageNumber = 1, int? pageSize = 10);
        Task<List<Product>> GetHighlightProducts(); 
        Task<Product?> GetProductByIdWithCategoriesAnsdDiscounts(Guid id);
        Task<Product?> GetProductById(Guid id);
        Task UpdateProduct (Product productFromDB);
        Task SaveProductPicture (Guid productId,  string picture);
        Task DeleteProductPicture(Guid productId);
        Task<bool> IsProductNameUniqueAsync(string name, Guid productId);
    }
}