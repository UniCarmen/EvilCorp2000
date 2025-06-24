using BusinessLayer.Models;
using static Shared.Utilities.Utilities;

namespace BusinessLayer.Services
{
    public interface IProductForSaleManager
    {
        Task<List<CategoryDTO>> GetCategories();
        Task<ProductForSaleDTO> GetProductForSale(Guid id);
        //Task<List<ProductForSaleDTO>> GetProductsForSale(ProductSortOrder? sortOrder = null, int? pageNumber = 1, int? pageSize = 10);
        Task<ProductListReturn<ProductForSaleDTO>> GetProductsForSale(GetProductsParameters parameters);
        Task<List<ProductForSaleDTO>> GetHighlightedProducts();
    }
}