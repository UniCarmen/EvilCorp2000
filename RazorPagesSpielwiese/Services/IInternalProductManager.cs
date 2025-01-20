using EvilCorp2000.Models;

namespace EvilCorp2000.Services
{
    public interface IInternalProductManager
    {
        Task<List<ProductForInternalUseDTO>> GetProductsForInternalUse();
        Task<ProductForInternalUseDTO> GetProductForInternalUse(Guid id);
        Task<List<CategoryDTO>> GetCategories();
        Task SaveProductToStore(ProductToStoreDTO productToStore);
        Task UpdateProductToStore(ProductToStoreDTO productToStore);
        Task AddDiscount(DiscountDTO discount, ProductToStoreDTO productToStore);
        Task SaveProductPicture(Guid productId, string encodedPicture);
        Task DeleteProductPicture(Guid productId);
        Task DeleteProduct(Guid productId);
    }
}