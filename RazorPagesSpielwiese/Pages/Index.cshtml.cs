using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        //private readonly ProductForSaleManager _productForSaleManager;


        public List<ProductForSaleDTO> ProductForSaleDTOs { get; set; }

        public IndexModel(ProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
        {
            //_productForSaleManager = productForSaleManager;
            _logger = logger;
        }

        
        public async Task OnGet()
        {
            try
            {
                ProductForSaleManager productForSaleManager = ProductForSaleManager.Create();
                ProductForSaleDTOs = await productForSaleManager.GetProductsForSale();
            }
            catch (Exception ex) {_logger.LogError("kein manager", ex);}
            
        }
    }
}
