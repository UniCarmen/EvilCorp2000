using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Pages.NewProduct;
using RazorPagesSpielwiese.Pages.ProductManagement;
using RazorPagesSpielwiese.Pages.ProductManagement.Partials;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages.ProductManagement
{
    public enum ShowModalState
    {
        None,
        New,
        Alter
    }

    public class ProductManagementModel : PageModel
    {
        //Enum Flags, welches Modal angezeigt werden soll
        //public Enum ShowModalState { get; set; }
        public ShowModalState ModalState { get; set; } = ShowModalState.None;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }


        //um es an das Partial weiterzugeben
        public Guid? SelectedProductId { get; set; }
        public ProductForInternalUseDTO SelectedProduct {get; set; }

        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public ProductForInternalUseDTO? Product { get; set; }

        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public List<Guid>? SelectedCategoryIds { get; set; }

        //NewProductValidation Test aus new Product
        [BindProperty]
        public NewProductViewModel1? ValidatedProduct { get; set; }

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


        public async Task<IActionResult> OnPostShowNewProductModal() 
        {
            ModalState = ShowModalState.New;
            await LoadDataAsync();
            return Page();
        }


        //was passiert, wenn nicht explizit der Schließen-Button gedrückt wird?
        public async Task<IActionResult> OnPostCloseModal()
        {
            ModalState = ShowModalState.None;
            await LoadDataAsync();
            return Page();
        }



        public async Task<IActionResult> OnPostShowAlterProductModal(Guid selectedProductId)
        {
            SelectedProductId = selectedProductId;
            await LoadDataAsync();
            var products = await _internalProductManager.GetProductsForInternalUse();
            SelectedProduct = products.FirstOrDefault(p => p.ProductId == selectedProductId);
            
            ModalState = ShowModalState.Alter;
            return Page();
            //muss hier das Product noch übergeben werden?
        }


        public async Task<IActionResult> OnPostSave()
        {

            //validation auch für altering product

            if (Product != null || ValidatedProduct != null)
            {

                var categories = await _internalProductManager.GetCategories();

                var selectedCategories = categories.FindAll(c => SelectedCategoryIds.Exists(id => id == c.CategoryId));


                //wie finde ich heraus, ob das product new ist oder nicht?
                //id vorhanden oder nicht

                var newProduct = new ProductToStoreDTO { };
                if(ValidatedProduct != null)
                {
                    //Problem bei alter product: amout on stock in validated Product ist null


                    newProduct.ProductName = ValidatedProduct.ProductName;
                    newProduct.ProductPicture = ValidatedProduct.ProductPicture;
                    newProduct.AmountOnStock = ValidatedProduct.AmountOnStock.Value;
                    newProduct.Description = ValidatedProduct.Description;
                    newProduct.Categories = selectedCategories;
                    //Discounts = Product.Discounts,
                    newProduct.Price = ValidatedProduct.Price.Value;

                    //gucken, dass update nur bei alterproduct
                    //und save product bei new product aufgerufen wird

                    await _internalProductManager.UpdateProductToStore(newProduct);
                }



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



            ModalState = ShowModalState.None;

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