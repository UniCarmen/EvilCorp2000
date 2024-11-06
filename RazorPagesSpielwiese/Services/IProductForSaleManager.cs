using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Services
{
    public interface IProductForSaleManager
    {
        Task<ProductForSaleDTO> GetProductForSale(Guid id);
        Task<List<ProductForSaleDTO>> GetProductsForSale();
    }
}