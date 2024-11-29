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
    }

    public class ProductManagementModel : PageModel
    {
        public ShowModalState ModalState { get; set; } = ShowModalState.None;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }



        //Weitergabe an Modal
        public Guid SelectedProductId { get; set; }

        //von Modal
        [BindProperty]
        public ValidatedProduct ValidatedProduct { get; set; }


        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger)
        {
            _internalProductManager = internalProductManager ?? throw new ArgumentNullException(nameof(internalProductManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnGet()
        {
            try
            {
                await LoadDataAsync();
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }


        public async Task<IActionResult> OnPostShowNewAndAlterProductModal(Guid selectedProductId) 
        {
            SelectedProductId = selectedProductId;
            await LoadDataAsync();

            if (selectedProductId != Guid.Empty)
            {
                try
                {
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
                catch (Exception ex) 
                {
                    _logger.LogError("Product not Found: {0}", ex);
                    ModalState = ShowModalState.None;
                    return Page();
                }
            }


            ModalState = ShowModalState.New;
            
            return Page();
        }


        //? TODO: was passiert, wenn nicht explizit der Schließen-Button gedrückt wird?
            // - bei Click außerhalb des Modals passiert nichts
            // - Button oben muss noch angepasst werden
            // - Button unten irgendwie schön gemacht, positioniert
        public async Task<IActionResult> OnPostCloseModal()
        {
            ModalState = ShowModalState.None;
            await LoadDataAsync();
            return Page();
        }

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

            ModalState = ShowModalState.None;
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