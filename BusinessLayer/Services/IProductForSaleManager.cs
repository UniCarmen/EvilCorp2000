using BusinessLayer.Models;
using static Shared.Utilities.Utilities;

namespace BusinessLayer.Services
{
    public interface IProductForSaleManager
    {
        Task<ProductForSaleDTO> GetProductForSale(Guid id);
        Task<List<ProductForSaleDTO>> GetProductsForSale(ProductSortOrder? sortOrder = null);
    }
}