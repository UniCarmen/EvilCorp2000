using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Services
{
    public interface IProductForSaleManager
    {
        Task<List<ProductForSaleDTO>> GetCategories();
        Task<ProductForSaleDTO> GetProductForSale(int id);
        Task<List<ProductForSaleDTO>> GetProductsForSale();
    }
}