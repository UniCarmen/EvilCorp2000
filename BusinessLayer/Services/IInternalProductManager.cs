using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public interface IInternalProductManager
    {
        Task<List<InternalProduct>> GetProductsForInternalUse();
        Task<InternalProduct> GetProductForInternalUse(Guid id);
        Task<List<CategoryDTO>> GetCategories();
        Task SaveProductToStore(InternalProduct productToStore);
        Task UpdateProductToStore(InternalProduct productToStore);
        Task AddDiscount(DiscountDTO discount, InternalProduct productToStore);
        Task SaveProductPicture(Guid productId, string encodedPicture);
        Task DeleteProductPicture(Guid productId);
        Task DeleteProduct(Guid productId);
    }
}