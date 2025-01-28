using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    public interface IProductForSaleManager
    {
        Task<ProductForSaleDTO> GetProductForSale(Guid id);
        Task<List<ProductForSaleDTO>> GetProductsForSale();
    }
}