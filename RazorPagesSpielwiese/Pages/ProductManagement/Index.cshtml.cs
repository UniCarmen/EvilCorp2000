using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;
using System.Text.Json;

namespace RazorPagesSpielwiese.Pages.ProductManagement
{
    

    public class ProductManagementModel : PageModel
    {
        public bool ShowModal { get; set; } = false;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? Products;
        public List<CategoryDTO>? Categories { get; set; }

        //[BindProperty]
        //public DiscountDTO? NewDiscount { get; set; }

        [BindProperty]
        public ValidatedDiscount? NewDiscount { get; set; }


        //Weitergabe an Modal
        public Guid SelectedProductId { get; set; }

        //von Modal
        [BindProperty]
        public ValidatedProduct ValidatedProduct { get; set; }

        [BindProperty]
        public string DiscountsJson { get; set; }

        [BindProperty]
        public string ValidatedProductJson { get; set; }

        [BindProperty]
        public string CategoryIdsJson { get; set; }


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
                    var categoryIds = selectedProduct.Categories.Select(c => c.CategoryId).ToList();

                    ValidatedProduct = new ValidatedProduct
                    {
                        ProductId = selectedProduct.ProductId,
                        ProductPicture = selectedProduct.ProductPicture,
                        ProductName = selectedProduct.ProductName,
                        AmountOnStock = selectedProduct.AmountOnStock,
                        SelectedCategoryIds = categoryIds,
                        Description = selectedProduct.Description,
                        Discounts = selectedProduct.Discounts,
                        Price = selectedProduct.Price
                    };

                    DiscountsJson = JsonSerializer.Serialize(selectedProduct.Discounts);
                    ValidatedProductJson = JsonSerializer.Serialize(selectedProduct);
                    CategoryIdsJson = JsonSerializer.Serialize(categoryIds);
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

            List<DiscountDTO> discounts = [] ;

            if (DiscountsJson != null)
                discounts = JsonSerializer.Deserialize<List<DiscountDTO>>(DiscountsJson);




                var newProduct = new ProductToStoreDTO
            {
                ProductName = ValidatedProduct.ProductName,
                ProductPicture = ValidatedProduct.ProductPicture,
                AmountOnStock = ValidatedProduct.AmountOnStock.Value,
                Description = ValidatedProduct.Description,
                Categories = selectedCategories,
                Discounts = discounts,
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
        public async Task<IActionResult> OnPostAddDiscount()
        {
            if (ModelState["NewDiscount.EndDate"]?.ValidationState != ModelValidationState.Valid ||
                ModelState["NewDiscount.StartDate"]?.ValidationState != ModelValidationState.Valid ||
                ModelState["NewDiscount.DiscountPercentage"]?.ValidationState != ModelValidationState.Valid ||
                ModelState["ValidatedProductJson"]?.ValidationState != ModelValidationState.Valid ||
                ModelState["CategoryIdsJson"]?.ValidationState != ModelValidationState.Valid)
            {
                
                
                //die seite wird zwar neu geladen ABER das product ist nicht mehr gefüllt...
                await LoadDataAsync();
                //SelectedProductId = ValidatedProduct.ProductId;
                //ValidatedProduct = ValidatedProduct;
                //ValidatedProductJson = ValidatedProductJson;
                //CategoryIdsJson = CategoryIdsJson;
                ShowModal = true;
                return Page();
            }

            await LoadDataAsync();

            var newDiscount = new DiscountDTO
            {
                StartDate = NewDiscount.StartDate.Value,
                EndDate = NewDiscount.EndDate.Value,
                DiscountPercentage = NewDiscount.DiscountPercentage.Value,
            };

            if (ValidatedProductJson != null)
                ValidatedProduct = JsonSerializer.Deserialize<ValidatedProduct>(ValidatedProductJson);

            //ich habe momentan nur eine guid liste....
            //die muss ich erst in eine dto liste mappen bzw die dtos raussuchen

            

            List<Guid> categoryIds = [];
            if (CategoryIdsJson != null) //prüfen schon mit modelstate
                categoryIds = JsonSerializer.Deserialize<List<Guid>>(CategoryIdsJson);

            //prüfen ob null

            var loadedCategories = await _internalProductManager.GetCategories();

            var selectedCategories = loadedCategories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId));

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

            await _internalProductManager.AddDiscount(newDiscount, newProduct);


            ValidatedProduct.SelectedCategoryIds = selectedCategories.Select(c => c.CategoryId).ToList();

            SelectedProductId = ValidatedProduct.ProductId;
            ValidatedProduct = ValidatedProduct;

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