using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages.ProductManagement
{
    

    public class ProductManagementModel : PageModel
    {
        public bool ShowModal { get; set; } = false;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }

        [BindProperty]
        public DiscountDTO? NewDiscount { get; set; }


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
                    ShowModal = false;
                    return Page();
                }
            }

            ShowModal = true;
            
            return Page();
        }


        public async Task<IActionResult> OnPostCloseModal()
        {
            ShowModal = false;
            await LoadDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostSaveProduct()
        {
            if (!ModelState.IsValid) 
            {
                ShowModal = true;
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

            ShowModal = false;
            return RedirectToPage();
        }



        //testDiscounts
        //funktioniert das, ohne dass das modal zugeht bzw. das modal wird wieder mit allen infos aufgemacht? -ne
        public async Task OnPostAddDiscount()
        {
            //validiert aber das ganze Model...das macht keinen Sinn
            //if (!ModelState.IsValid)
            //{
            //    return Page(); // Validierungsfehler bleiben im Modal sichtbar
            //}

            //Prüfen, ob der NewDiscount Valide ist
            // Neuen Discount hinzufügen
            var newDiscount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                StartDate = NewDiscount.StartDate,
                EndDate = NewDiscount.EndDate,
                DiscountPercentage = NewDiscount.DiscountPercentage
            };
            ValidatedProduct.Discounts.Add(newDiscount);

            await OnPostReloadModalWithAlteredDiscounts(ValidatedProduct);

            //ShowModal = true;
            //await LoadDataAsync();
            //return Page();

        }


        public async Task<IActionResult> OnPostReloadModalWithAlteredDiscounts(ValidatedProduct validatedProduct)
        {
            SelectedProductId = validatedProduct.ProductId;
            ValidatedProduct = validatedProduct;
            await LoadDataAsync();

            ShowModal = true;

            return Page();
        }


        //bei den unteren muss ich noch gucken, wie ich das zum Laufen bekomme
        public IActionResult OnPostEditDiscount(Guid discountId)
        {
            // Discount bearbeiten (Logik zur Anzeige der Edit-Werte)
            return Page();
        }

        public IActionResult OnPostDeleteDiscount(Guid discountId)
        {
            // Discount löschen
            ValidatedProduct.Discounts.RemoveAll(d => d.DiscountId == discountId);
            return Page();
        }

        public IActionResult OnPostDismiss()
        {
            // Logik beim Dismiss-Button
            ViewData["ModalDismissed"] = true; // Beispielhafte Änderung
            return Page();
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