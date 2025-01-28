using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DataAccess.Repositories;

namespace EvilCorp2000.Pages
{
    public class ShopMainModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;


        public List<ProductForSaleDTO> ProductsForSale { get; set; }


        public ShopMainModel(IProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _productForSaleManager = productForSaleManager;
        }


        public async Task OnGet()
        {
            try
            {
                ProductsForSale = await _productForSaleManager.GetProductsForSale();
            }
            catch (Exception ex) { _logger.LogError(ex, "Error getting the products from the database"); }

        }


    }
}
