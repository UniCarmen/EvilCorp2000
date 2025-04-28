using BusinessLayer.Models;
using static Shared.Utilities.Utilities;

namespace BusinessLayer.Services
{
    public interface IInternalProductManager
    {
        Task<ProductListReturn<ProductManagementProductDTO>> GetProductsForInternalUse(ProductSortOrder? sortOrderString = null, int? pageNumber = 1, int? pageSize = 10);
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