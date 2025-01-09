using EvilCorp2000.Models;

namespace EvilCorp2000.Services
{
    public interface IProductForSaleManager
    {
        Task<ProductForSaleDTO> GetProductForSale(Guid id);
        Task<List<ProductForSaleDTO>> GetProductsForSale();
    }
}