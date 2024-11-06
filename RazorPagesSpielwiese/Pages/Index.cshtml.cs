using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Repositories;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;


        public List<ProductForSaleDTO> ProductsForSale { get; set; }


        public IndexModel(IProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
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
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte", ex); }

        }


    }
}
