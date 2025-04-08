using BusinessLayer.Models;
using BusinessLayer.Services;
using DataAccess.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Serilog;
using static Shared.Utilities.Utilities;

namespace EvilCorp2000.Pages
{
    public class ShopViewModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;


        public List<ProductForSaleDTO> ProductsForSale { get; private set; } = [];
        public string SortOrder { get; set; } = "Default";
        public List<string> SortOrderString { get;  } = [
            ProductSortOrder.Default.ToString(),
            ProductSortOrder.PriceAsc.ToString(),
            ProductSortOrder.PriceDesc.ToString(),
            ProductSortOrder.DiscountDesc.ToString(),
            ProductSortOrder.DiscountAsc.ToString()];

        public ShopViewModel(IProductForSaleManager productForSaleManager, ILogger<IndexModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productForSaleManager = productForSaleManager ?? throw new ArgumentNullException(nameof(productForSaleManager));
        }


        public async Task<IActionResult> OnGet(string? sortOrderString = null)
        {
            try
            {
                SortOrder = sortOrderString ?? "Default";
                if (Enum.TryParse<ProductSortOrder>(sortOrderString, out var sortOrder))
                {
                    ProductsForSale = await _productForSaleManager.GetProductsForSale(sortOrder);
                }
                else { ProductsForSale = await _productForSaleManager.GetProductsForSale(); }


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


        public async Task<IActionResult> OnPostSort (string sortOrder)
        {
            SortOrder = sortOrder; // Den Wert speichern, der vom Formular gesendet wurde
            //neues loadSortedProducts - oder mit optionalem parameter in onget (enum oder so, welcher sortierungsreihenfolge)
            //Backend SortedLoadingFunctionen und als Liste zurückgeben (async)

            //sollen dann so geladen und in ProductsForSale

            return Page();
        }


        private void LogAndAddModelError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
            ModelState.AddModelError(string.Empty, message);
        }


    }
}
