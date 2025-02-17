using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public interface IInternalProductManager
    {
        Task<List<ProductManagementProductDTO>> GetProductsForInternalUse();
        Task<ProductManagementProductDTO> GetProductForInternalUse(Guid id);
        Task<List<CategoryDTO>> GetCategories();
        Task SaveProductToStore(ProductManagementProductDTO productToStore);
        Task UpdateProductToStore(ProductManagementProductDTO productToStore);
        Task AddDiscount(DiscountDTO discount, ProductManagementProductDTO productToStore);
        Task SaveProductPicture(Guid productId, string encodedPicture);
        Task DeleteProductPicture(Guid productId);
        Task DeleteProduct(Guid productId);
    }
}