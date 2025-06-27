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
using static EvilCorp2000.Pages.Utilities.Utilities;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace EvilCorp2000.Pages
{
    public class ShopViewModel : PageModel
    {
        private readonly ILogger<ShopViewModel> _logger;
        private readonly IProductForSaleManager _productForSaleManager;

        public int CountProducts { get; set; }
        public int MaxPageCount { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string? Search {  get; set; }
        public Guid? FilterCategory {  get; set; }
        public string? FilterCategoryString { get; set; } // wird für das Dropdown benötigt
        public List<CategoryDTO>? _categories { get; set; }


        public List<ProductForSaleDTO> ProductsForSale { get; private set; } = [];
        public string SortOrderString { get; set; } = "Default";

        public List<(string, string)> SortOrderAndDisplayStrings { get; } = [
            (ProductSortOrder.Default.ToString(), "Default"),
            (ProductSortOrder.PriceAsc.ToString(), "Price Ascending"),
            (ProductSortOrder.PriceDesc.ToString(), "Price Descending"),
            (ProductSortOrder.DiscountDesc.ToString(), "Discount Descending"),
            (ProductSortOrder.DiscountAsc.ToString(), "Discount Ascending"),
            (ProductSortOrder.RatingDesc.ToString(), "Rating")
            ];

        public ShopViewModel(IProductForSaleManager productForSaleManager, ILogger<ShopViewModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _productForSaleManager = productForSaleManager ?? throw new ArgumentNullException(nameof(productForSaleManager));
        }


        public async Task<IActionResult> OnGet(UIGetProductsParameters parameters)
        {
            try
            {
                _categories = await _productForSaleManager.GetCategories();

                PageNumber = (parameters.PageNumber.HasValue && parameters.PageNumber.Value > 0) ? parameters.PageNumber.Value : 1;
                PageSize = (parameters.PageSize.HasValue && parameters.PageSize.Value > 0) ? parameters.PageSize.Value : 10;
                SortOrderString = parameters.SortOrderString ?? "Default";
                Search = parameters.Search ?? "";
                if (!parameters.FilterCategoryString.IsNullOrEmpty() && !_categories.IsNullOrEmpty())
                {
                    FilterCategory =
                    _categories
                    .Find(c => c.CategoryId == Guid.Parse(parameters.FilterCategoryString)).CategoryId;
                    FilterCategoryString = parameters.FilterCategoryString;
                    //hier muss ich anhanf FilterCAtegoryString (GUID) die id (richtige Guid raussuchen)
                    //parameters.FilterCategory;
                }



                GetProductsParameters parametersWithSortOrder = new GetProductsParameters()
                {
                    SortOrder = MapSortOrderString(SortOrderString),
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    CategoryId = FilterCategory,
                    Search = Search
                };

                

                var productListReturn = await _productForSaleManager.GetProductsForSale(
                    parametersWithSortOrder);

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


        //public async Task<IActionResult> OnPostSort (string sortOrder)
        //{
        //    SortOrderString = sortOrder; // Den Wert speichern, der vom Formular gesendet wurde
        //    //neues loadSortedProducts - oder mit optionalem parameter in onget (enum oder so, welcher sortierungsreihenfolge)
        //    //Backend SortedLoadingFunctionen und als Liste zurückgeben (async)

        //    //sollen dann so geladen und in ProductsForSale

        //    return Page();
        //}


        private void LogAndAddModelError(string message, Exception ex)
        {
            _logger.LogError(ex, message);
            ModelState.AddModelError(string.Empty, message);
        }


    }
}
