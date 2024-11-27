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
        public ShowModalState ModalState { get; set; } = ShowModalState.None;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }



        //Weitergabe an Partial
        public Guid? SelectedProductId { get; set; }
        public ProductForInternalUseDTO SelectedProduct {get; set; }

        //TODO: muss weg, dafür soll ValidatedProduct verwendet werden
        //um die Daten aus dem Partial zu bekommen
        [BindProperty]
        public ProductForInternalUseDTO? Product { get; set; }

        // Rückgabe aus dem Partial 
        //? warum ist es nullable?
        [BindProperty]
        public ValidatedProduct? ValidatedProduct { get; set; }


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


        //? was passiert, wenn nicht explizit der Schließen-Button gedrückt wird?
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
        }


        public async Task<IActionResult> OnPostSave()
        {
            //wenn ModelState nicht valid ist, dann... neu laden mit eingegebenen Produktdaten und Fehleranzeige
            if (!ModelState.IsValid) 
            {
                ModalState = ShowModalState.New;
                await LoadDataAsync();

                var temporarySelectedCategories = Categories.Where(c => ValidatedProduct.SelectedCategoryIds.Contains(c.CategoryId)).ToList();

                decimal price;
                if (ValidatedProduct.Price == null)
                    price = 0.0m;
                else price = ValidatedProduct.Price.Value;

                int amountOnStock;
                if (ValidatedProduct.AmountOnStock == null)
                    amountOnStock = 0;
                else amountOnStock = ValidatedProduct.AmountOnStock.Value;

                SelectedProduct = new ProductForInternalUseDTO 
                {
                    ProductPicture = ValidatedProduct.ProductPicture,
                    ProductName = ValidatedProduct.Description,
                    Description = ValidatedProduct.Description,
                    Price = price,
                    AmountOnStock = amountOnStock,
                    Discounts = ValidatedProduct.Discounts,
                    Categories = temporarySelectedCategories
                };
                return Page();
            }

            //so, als Vorbereitung zum speichern eines AlterProducts habe ich eine Id in ValidatedProduct eingefügt
            //hier kann geprüft werden, ob es sich um ein NEUES Product oder eines zum UPDATEN handelt
            //die Id wird bei einem neuen Product als Guid.Empts mitgegeben

            var categories = await _internalProductManager.GetCategories();

            var selectedCategories = categories.FindAll(c => ValidatedProduct.SelectedCategoryIds.Exists(id => id == c.CategoryId));

            var newProduct = new ProductToStoreDTO
            {
                ProductName = ValidatedProduct.ProductName,
                ProductPicture = ValidatedProduct.ProductPicture,
                AmountOnStock = ValidatedProduct.AmountOnStock.Value,
                Description = ValidatedProduct.Description,
                Categories = selectedCategories,
                Discounts = ValidatedProduct.Discounts,
                Price = ValidatedProduct.Price.Value
            };

            if (ValidatedProduct.ProductId == Guid.Empty)
            {
                //Speichern eines NEUEN ProductToStoreDTO
                await _internalProductManager.SaveProductToStore(newProduct);
            }

            else
            {
                //hier muss die ProductId von newProduct noch gefüllt werden
                await _internalProductManager.UpdateProductToStore(newProduct);
            }
            

            





            //    //TODO: hier soll in Zukunft nur noch auf Validated Product geprüft werden
            //    if (Product != null || ValidatedProduct != null)
            //{

            //    var categories = await _internalProductManager.GetCategories();

            //    var selectedCategories = categories.FindAll(c => SelectedCategoryIds.Exists(id => id == c.CategoryId));


            //    //TODO: ValidatedProduct auch bei Alter Product verwenden
            //    //über das ValidatedProduct auch eine nullabe Id mitgeben.
            //    //Falls Id vorhanden dann ist es ein AlterPRoduct ->UpdateProductToStoreFunktion
            //    //ohne Id: SaveProduct

            //    var newProduct = new ProductToStoreDTO { };
            //    if(ValidatedProduct != null)
            //    {
            //        //Problem bei alter product: amout on stock in validated Product ist null

            //        newProduct.ProductName = ValidatedProduct.ProductName;
            //        newProduct.ProductPicture = ValidatedProduct.ProductPicture;
            //        newProduct.AmountOnStock = ValidatedProduct.AmountOnStock.Value;
            //        newProduct.Description = ValidatedProduct.Description;
            //        newProduct.Categories = selectedCategories;
            //        //Discounts = Product.Discounts,
            //        newProduct.Price = ValidatedProduct.Price.Value;

            //        await _internalProductManager.UpdateProductToStore(newProduct);
            //    }



            //    var productToStore = new ProductToStoreDTO
            //    {
            //        ProductId = Product.ProductId,
            //        ProductName = Product.ProductName,
            //        ProductPicture = Product.ProductPicture,
            //        AmountOnStock = Product.AmountOnStock,
            //        Categories = selectedCategories,
            //        Description = Product.Description,
            //        Discounts = Product.Discounts,
            //        Price = Product.Price,
            //        Rating = Product.Rating
            //    };

            //    //validation

            //    await _internalProductManager.UpdateProductToStore(productToStore);

            //}



            //ModalState = ShowModalState.None;

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