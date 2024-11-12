using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Pages.Shared;
using RazorPagesSpielwiese.Pages.Shared.Partials;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages.ProductManagement
{
    public class ProductManagementModel : PageModel
    {

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }

        //um es an das Partial weiterzugeben
        public Guid? SelectedProductId { get; set; }

        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public ProductForInternalUseDTO? Product { get; set; }

        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public List<Guid>? SelectedCategoryIds { get; set; }

        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger)
        {
            _internalProductManager = internalProductManager ?? throw new ArgumentNullException(nameof(internalProductManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task OnGet(Guid? selectedProduct)
        {
            try
            {
                await LoadDataAsync();
                SelectedProductId = selectedProduct;
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }



        public async Task<IActionResult> OnPostSave()
        {
            if (Product != null)
            {
                var categories = await _internalProductManager.GetCategories();

                var selectedCategories = categories.FindAll(c => SelectedCategoryIds.Exists(id => id == c.CategoryId));

                var productToStore = new ProductToStoreDTO
                {
                    ProductId = Product.ProductId,
                    ProductName = Product.ProductName,
                    ProductPicture = Product.ProductPicture,
                    AmountOnStock = Product.AmountOnStock,
                    Categories = selectedCategories,
                    Description = Product.Description,
                    Discounts = Product.Discounts,
                    Price = Product.Price,
                    Rating = Product.Rating
                };

                //validation

                await _internalProductManager.UpdateProductToStore(productToStore);
            }

            

            

            //soll aber neu geladen werden
            return RedirectToPage();
        }


        public async Task LoadDataAsync()
        {
            try
            {
                Products = await _internalProductManager.GetProductsForInternalUse();
                Categories = await _internalProductManager.GetCategories();
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }
    }
}