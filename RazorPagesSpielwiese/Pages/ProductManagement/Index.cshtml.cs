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

        //Produkte prop
        public List<ProductForInternalUseDTO> Products;
        public List<CategoryDTO> Categories { get; set; }

        //[BindProperty]
        //public AlterProductModalPartialModel? ModalData { get; set; }

        //um es an das Partial weiterzugeben
        public Guid? SelectedProductId { get; set; }

        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public ProductForInternalUseDTO Product { get; set; }

        //[BindProperty]
        //public List<CategoryDTO> SelectedCategories { get; set; }

        [BindProperty]
        public List<Guid> SelectedCategoryIds { get; set; }

        ////eigentlich nicht nötig, da ich den Typ so an die Funktion übergeben kann
        //public ProductToStoreDTO ProductToStore { get; set; }

        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger)
        {
            _internalProductManager = internalProductManager;
            _logger = logger;
        }


        public async Task OnGet(Guid? selectedProduct)
        {
            try
            {
                //TODO:
                //ViewModel ProductToChange, mit gewissen Inhalten als Strings, UI Object
                //Model ProductToChangeDTO, Business Object

                //ANZEIGEN
                //Produkte = await _internalProductManager.
                //InterneProdukte abrufen, anzeigen als Liste, mit Discount Infos (Datum)

                await LoadDataAsync();
                SelectedProductId = selectedProduct;
                //TODO: in mappings

                
                


                //VERÄNDERN
                //sie verändern, //Möglichkeit, Bestand zu ändern, Beschreibung, etc.
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }

        //public async Task<IActionResult> OnPostSelectProductId(Guid selectedProductId)
        //{
        //    await LoadDataAsync();
        //    var categories = Categories;
        //    SelectedProductId = selectedProductId;

        //    var modalModel = new AlterProductModalPartialModel
        //    {
        //        Product = Products.FirstOrDefault(p => p.ProductId == selectedProductId),
        //        Categories = categories
        //    };

        //    // Gib das Partial View mit den geladenen Daten zurück
        //    return Partial("Shared/Partials/AlterProductModalPartial", modalModel);

        //    //return Partial("Shared/Partials/AlterProductModalPartial",
        //    //    new AlterProductModalPartialModel { Product = Products.FirstOrDefault(p => p.ProductId == selectedProductId), Categories = Categories });
        //}

        public async Task<IActionResult> OnPostSave()
        {
            //mapping internaluse product -> product to store


            if (Product != null)
            {
                //get right categories from selectedCategoryIds from the Modal
                //SelectedCategoryIds
                //load Categorys

                var categories = await _internalProductManager.GetCategories();

                var selectedCategories = categories.FindAll(c => SelectedCategoryIds.Exists(id => id == c.CategoryId));

                var productToStore = new ProductToStoreDTO
                {
                    //wird nicht richtig übergeben = GuidEmpty
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