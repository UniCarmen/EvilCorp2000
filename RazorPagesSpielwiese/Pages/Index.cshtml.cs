using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;

        public List<ProductForSaleDTO> ProductForSale { get; set; }


        public IndexModel(IProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
        {
            //_productForSaleManager = productForSaleManager;
            _logger = logger;
            _productForSaleManager = productForSaleManager;
        }

        
        public async Task OnGet()
        {
            try
            {
                ProductForSale = await _productForSaleManager.GetProductsForSale();
            }
            catch (Exception ex) {_logger.LogError("Fehler beim Abrufen der Produkte", ex);}
            
        }


    }
}
