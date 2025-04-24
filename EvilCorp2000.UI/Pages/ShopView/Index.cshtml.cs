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

        public int CountProducts { get; set; }
        public int MaxPageCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }

        public List<ProductForSaleDTO> ProductsForSale { get; private set; } = [];
        public string SortOrder { get; set; } = "Default";

        public List<(string, string)> SortOrderAndDisplayStrings { get; } = [
            (ProductSortOrder.Default.ToString(), "Default"),
            (ProductSortOrder.PriceAsc.ToString(), "Price Ascending"),
            (ProductSortOrder.PriceDesc.ToString(), "Price Descending"),
            (ProductSortOrder.DiscountDesc.ToString(), "Discount Descending"),
            (ProductSortOrder.DiscountAsc.ToString(), "Discount Ascending"),
            (ProductSortOrder.RatingDesc.ToString(), "Rating")
            ];

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

                ProductListReturn<ProductForSaleDTO> productListReturn;

                if (Enum.TryParse<ProductSortOrder>(sortOrderString, out var sortOrder))
                {
                    productListReturn = await _productForSaleManager.GetProductsForSale(sortOrder);
                }
                else { productListReturn = await _productForSaleManager.GetProductsForSale(); }

                ProductsForSale = productListReturn.ProductList;

                CountProducts = productListReturn.ProductCount;

                MaxPageCount = productListReturn.MaxPageCount;


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
