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

        public List<ProductForSaleDTO> RandomDiscountedProducts { get; set; } = [];

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

                var discountedProducts = ProductsForSale.Where(p => p.DiscountedPrice.HasValue).ToList();
                if (discountedProducts.Count() >= 2)
                {
                    var random = new Random();
                    var randomDiscountedProducts = discountedProducts
                        .OrderBy(_ => random.Next())
                        .Take(2)
                        .ToList();
                    RandomDiscountedProducts = randomDiscountedProducts;
                };
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
