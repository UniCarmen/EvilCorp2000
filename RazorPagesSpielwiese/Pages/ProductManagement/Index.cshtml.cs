using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
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
        //? wofür nochmal die Id
        public Guid? SelectedProductId { get; set; }


        // Rückgabe aus dem Partial 
        //? warum ist es nullable?
        [BindProperty]
        public ValidatedProduct ValidatedProduct { get; set; }


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


        public async Task<IActionResult> OnPostShowNewProductModal(Guid selectedProductId) 
        {
            SelectedProductId = selectedProductId;
            await LoadDataAsync();

            if (selectedProductId != Guid.Empty)
            {
                await LoadDataAsync();

                var selectedProduct = Products.FirstOrDefault(p => p.ProductId == selectedProductId);

                ValidatedProduct = new ValidatedProduct
                {
                    ProductId = selectedProduct.ProductId,
                    ProductPicture = selectedProduct.ProductPicture,
                    ProductName = selectedProduct.ProductName,
                    AmountOnStock = selectedProduct.AmountOnStock,
                    SelectedCategoryIds = selectedProduct.Categories.Select(c => c.CategoryId).ToList(),
                    Description = selectedProduct.Description,
                    Discounts = selectedProduct.Discounts,
                    Price = selectedProduct.Price
                };
            }


            ModalState = ShowModalState.New;
            
            return Page();
        }


        //? TODO: was passiert, wenn nicht explizit der Schließen-Button gedrückt wird?
        public async Task<IActionResult> OnPostCloseModal()
        {
            ModalState = ShowModalState.None;
            await LoadDataAsync();
            return Page();
        }



        //public async Task<IActionResult> OnPostShowAlterProductModal(Guid selectedProductId)
        //{
        //    SelectedProductId = selectedProductId;
        //    await LoadDataAsync();
        //    var products = await _internalProductManager.GetProductsForInternalUse();
        //    SelectedProduct = products.FirstOrDefault(p => p.ProductId == selectedProductId);
            
        //    //ModalState = ShowModalState.Alter;
        //    ModalState = ShowModalState.New;
        //    return Page();
        //}


        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid) 
            {
                ModalState = ShowModalState.New;
                await LoadDataAsync();
                return Page();
            }

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
                Price = ValidatedProduct.Price.Value,
                ProductId = ValidatedProduct.ProductId,
            };

            if (ValidatedProduct.ProductId == Guid.Empty)
            {
                await _internalProductManager.SaveProductToStore(newProduct);
            }

            else
            {
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