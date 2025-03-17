using BusinessLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EvilCorp2000.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;


        public List<ProductForSaleDTO> ProductsForSale { get; private set; } = [];


        public IndexModel(IProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productForSaleManager = productForSaleManager ?? throw new ArgumentNullException(nameof(productForSaleManager));
        }


        public async Task<IActionResult> OnGet()
        {
            try
            {
                ProductsForSale = await _productForSaleManager.GetProductsForSale();
            }
            catch (DbUpdateException ex)
            {
                LogAndAddModelError("Fehler in der Datenbank", ex);
            }
            catch (Exception ex)
            {
                LogAndAddModelError("Fehler beim Laden der Produkte.", ex);
            }

            return Page();
        }


        private void LogAndAddModelError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
            ModelState.AddModelError(string.Empty, message);
        }


    }
}
