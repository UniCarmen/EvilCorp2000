using EvilCorp2000.Models;
using EvilCorp2000.Pages.ProductManagement.Partials;
using EvilCorp2000.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using EvilCorp2000.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design.Serialization;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using Microsoft.Identity.Client;
using static System.Net.Mime.MediaTypeNames;
using SixLabors.ImageSharp;

namespace EvilCorp2000.Pages.ProductManagement
{


    public class ProductManagementModel : PageModel
    {
        public bool ShowModal { get; set; } = false;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        public List<ProductForInternalUseDTO>? products;

        [BindProperty]
        public IFormFile ImageFile { get; set; }

        public List<CategoryDTO>? Categories { get; set; }

        [BindProperty]
        public ValidatedDiscount? NewDiscount { get; set; }

        public Guid SelectedProductId { get; set; }

        [BindProperty]
        public ValidatedProduct ValidatedProduct { get; set; }

        [BindProperty]
        public string DiscountsJson { get; set; }

        [BindProperty]
        public bool DiscountOverlap { get; set; } // = false;

        [BindProperty]
        public string ValidatedProductJson { get; set; }

        [BindProperty]
        public string CategoryIdsJson { get; set; }

        public NewProductModalPartialModel PartialModel
        {
            get => new NewProductModalPartialModel
            {
                ValidatedProduct = ValidatedProduct,
                Categories = Categories,
                DiscountsJson = DiscountsJson,
                ValidatedProductJson = ValidatedProductJson,
                CategoryIdsJson = CategoryIdsJson,
                DiscountOverlap = DiscountOverlap,
                NewDiscount = new ValidatedDiscount()
            };
            set { }
        }


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
                    var selectedProduct = products.FirstOrDefault(p => p.ProductId == selectedProductId);
                    var categoryIds = selectedProduct.Categories.Select(c => c.CategoryId).ToList();


                    var validatedProduct = CreateValidatedProduct(selectedProduct, categoryIds);


                    if (!validatedProduct.Discounts.IsNullOrEmpty())
                    {
                        validatedProduct.Discounts = validatedProduct.Discounts.OrderBy(d => d.StartDate).ToList();
                    }

                    ValidatedProduct = validatedProduct;

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
            if (!IsModelStateValidForProduct(ValidatedProduct.ProductId))
            {
                ShowModal = true;
                await LoadDataAsync();
                return await ReInitializeModalWithProduct(ValidatedProduct, ValidatedProduct.SelectedCategoryIds, ValidatedProduct.Discounts);
            }


            //Zusammenstellen des ProductToStoreDTO

            List<CategoryDTO> categories;

            try
            {
                categories = await _internalProductManager.GetCategories();
            }

            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Failed to load categories. {0}", "Failed to load categories.", ex);
            }

            var selectedCategories = categories.FindAll(c => ValidatedProduct.SelectedCategoryIds.Exists(id => id == c.CategoryId));

            if (DiscountsJson != null)
            {
                (List<DiscountDTO> deserializedDiscounts, IActionResult? discountDTOJsonError) =
                    await DeserializeWithTryCatchAsync<List<DiscountDTO>>(DiscountsJson, "Failed to parse DiscountDTO List.", "Product couldn't be added.");
                if (discountDTOJsonError != null) return discountDTOJsonError;
                ValidatedProduct.Discounts = deserializedDiscounts;
            }

            var newProduct = CreateProductToStoreDTO(ValidatedProduct, selectedCategories);


            //Speichern des ProductToStoreDTO
            try
            {
                if (ValidatedProduct.ProductId == Guid.Empty)
                {
                    await _internalProductManager.SaveProductToStore(newProduct);
                }

                else
                {
                    await _internalProductManager.UpdateProductToStore(newProduct);
                }
            }

            catch (ValidationException ex)
            {
                // Fehlermeldung zur ModelState hinzufügen
                ModelState.AddModelError("ProductValidation", ex.Message);
                return await ReInitializeModalWithProduct(ValidatedProduct, ValidatedProduct.SelectedCategoryIds, ValidatedProduct.Discounts);
            }

            catch (Exception ex)
            {

                return await ExecuteOnExceptionCatch("Failed to save the product. {0}", "Failed to save the product.", ex);
            }

            ShowModal = false;
            return RedirectToPage();
        }


        public async Task<IActionResult> OnPostAddDiscount()
        {
            if (IsModelStateIsInvalidForDiscount(ModelState))
            {
                await LoadDataAsync();
                return await ReInitializeModalAfterDiscountValidationError(CategoryIdsJson, Categories, ValidatedProductJson);
            }

            await LoadDataAsync();

            var newDiscount = new DiscountDTO
            {
                StartDate = NewDiscount.StartDate.Value,
                EndDate = NewDiscount.EndDate.Value,
                DiscountPercentage = NewDiscount.DiscountPercentage.Value,
            };


            // Load Product from DB to get all Discounts, if one has already been entered            
            (ValidatedProduct validatedProduct, IActionResult? validatedProductError) =
                await DeserializeWithTryCatchAsync<ValidatedProduct>(ValidatedProductJson, "Failed to parse ValidatedProduct.", "Discount couldn't be added.");
            if (validatedProductError != null) return validatedProductError;
            ValidatedProduct = validatedProduct;

            var newSelectedProduct = products.FirstOrDefault(p => p.ProductId == ValidatedProduct.ProductId);
            ValidatedProduct.Discounts = newSelectedProduct.Discounts;

            // Load SelectedCategories to fill Fields after Reload of the Modal after Saving the Discount - if not done, there are na categories in the product
            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            var selectedCategories = Categories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId));



            var newProduct = CreateProductToStoreDTO(ValidatedProduct, selectedCategories);

            try
            {
                //separate function because a guid is added in the backend
                await _internalProductManager.AddDiscount(newDiscount, newProduct);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("DiscountValidation", ex.Message);
                return await ReInitializeModalWithProduct(ValidatedProduct, ValidatedProduct.SelectedCategoryIds, ValidatedProduct.Discounts);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be added.", ex);
            }


            // neues Product mit allen Discounts inkl. dem Neuem Laden
            var selectedProduct = products.FirstOrDefault(p => p.ProductId == ValidatedProduct.ProductId);

            return await ReInitializeModalWithProduct(ValidatedProduct, categoryIds, selectedProduct.Discounts);

        }


        private async Task<IActionResult> ModelError(string Message, ValidatedProduct validatedProduct, List<Guid> categoryIds, List<DiscountDTO> discountDtos)
        {
            ModelState.AddModelError(string.Empty, "Please select a valid image file.");
            return await ReInitializeModalWithProduct(ValidatedProduct, categoryIds, discountDtos);
        }



        public async Task<IActionResult> OnPostImageUpload()
        {
            //TODO: ich muss diese Hiddenfields einfügen, damit im Falle eines ModelErrors die Felder wieder gefüllt werden können: Validated Product, CategoryIds, Discounts

            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            (ValidatedProduct validatedProduct, IActionResult? validatedProductError) =
                await DeserializeWithTryCatchAsync<ValidatedProduct>(ValidatedProductJson, "Failed to parse ValidatedProduct.", "Discount couldn't be added.");
            if (validatedProductError != null) return validatedProductError;

            (List<DiscountDTO> deserializedDiscounts, IActionResult? discountDTOJsonError) =
                    await DeserializeWithTryCatchAsync<List<DiscountDTO>>(DiscountsJson, "Failed to parse DiscountDTO List.", "Product couldn't be added.");
            if (discountDTOJsonError != null) return discountDTOJsonError;


            //TODO: Validations -> auslagern?
            if (ImageFile == null || ImageFile.Length == 0)
            {
                await ModelError("Please select a valid image file.", validatedProduct, categoryIds, deserializedDiscounts);
            }

            // Dateigrößenprüfung (z. B. maximal 2 MB)
            if (ImageFile.Length > 2 * 1024 * 1024) // 2 MB
            {
                await ModelError("The file size exceeds the 2 MB limit.", validatedProduct, categoryIds, deserializedDiscounts);
            }

            // MIME-Typ-Prüfung
            var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/gif" };

            if (!allowedFileTypes.Contains(ImageFile.ContentType))
            {
                await ModelError("Only JPEG, PNG, and GIF formats are allowed.", validatedProduct, categoryIds, deserializedDiscounts);
            }

            // Dateierweiterung prüfen
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                await ModelError("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.", validatedProduct, categoryIds, deserializedDiscounts);
            }

            // Bildinhalt validieren --> ins Backend? bzw. den Service??
            try
            {
                using var image = SixLabors.ImageSharp.Image.Load(ImageFile.OpenReadStream());
            }
            catch
            {
                await ModelError("The uploaded file is not a valid image.", validatedProduct, categoryIds, deserializedDiscounts);
            }

            string productPictureToSave;

            // Konvertiere das Bild in einen Base64-String
            using (var memoryStream = new MemoryStream())
            {
                await ImageFile.CopyToAsync(memoryStream);
                var imageBytes = memoryStream.ToArray();
                productPictureToSave = Convert.ToBase64String(imageBytes);
            }

            validatedProduct.ProductPicture = productPictureToSave;

            try
            {
                await _internalProductManager.SaveProductPicture(validatedProduct.ProductId, productPictureToSave);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be added.", ex);
            }

            //TODO: Rückmeldung anzeigen, ob geklappt oder nicht?
            //Seite mit allem neu laden / bzw. auch mit Modal
            return await ReInitializeModalWithProduct(validatedProduct, categoryIds, deserializedDiscounts);
            //return Page();
        }


        public async Task<IActionResult> OnPostDeleteProduct(Guid productId)
        {
            //TODO: Rückmeldung anzeigen, ob geklappt oder nicht?
            try
            {
                await _internalProductManager.DeleteProduct(productId);
                ShowModal = false;
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Failed to delete the product. {0}", "Failed to delete the product.", ex);
            }
        }





        public async Task<IActionResult> OnPostDeleteDiscount(Guid discountId, Guid productId) // hier gleich auch die Product Id mitgeben
        {

            // productliste in der Hauptseite nicht mehr sichtbar


            //Product neu laden, da es Probleme gibt, wenn ich mit dem alten ValidatedProduct weiterarbeiten würde
            var loadedProduct = await _internalProductManager.GetProductForInternalUse(productId);

            var selectedCategories = loadedProduct.Categories.Select(c => c.CategoryId).ToList();

            //var IFormFile image =
            //convert loadedProduct.ProductPicture to IFile irgendwas

            var newValidatedProduct = new ValidatedProduct
            {
                ProductId = loadedProduct.ProductId,
                AmountOnStock = loadedProduct.AmountOnStock,
                Description = loadedProduct.Description,
                Discounts = loadedProduct.Discounts,
                Price = loadedProduct.Price,
                ProductName = loadedProduct.ProductName,
                ProductPicture = loadedProduct.ProductPicture,
                SelectedCategoryIds = selectedCategories
            };

            ValidatedProduct = newValidatedProduct;

            // Discount löschen from List
            try
            {
                var newDisounts = ValidatedProduct.Discounts.FindAll(d => d.DiscountId != discountId);
                ValidatedProduct.Discounts = newDisounts;
            }

            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Failed to remove Discount from Discounts", "An error occured", ex);
            }

            //wird in Reinitialize gemacht
            DiscountsJson = JsonSerializer.Serialize(ValidatedProduct.Discounts);
            ValidatedProductJson = JsonSerializer.Serialize(ValidatedProduct);

            // Load SelectedCategories to fill Fields after Reload of the Modal after Saving the Discount
            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            //Categories = await _internalProductManager.GetCategories(); --> beim Reinitialize geladen

            //var selectedCategories = Categories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId)); --> oben geändert

            var productToStore = CreateProductToStoreDTO(ValidatedProduct, loadedProduct.Categories);//brauche ich die categories, sind die im ValidatedProducts leer?

            //neues Product speichern
            try
            {
                await _internalProductManager.UpdateProductToStore(productToStore);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be deleted.", ex);
            }

            //soll ich das Product neu laden???
            //var products = await _internalProductManager.GetProductsForInternalUse();
            //var selectedProduct = products.FirstOrDefault(p => p.ProductId == ValidatedProduct.ProductId);

            return await ReInitializeModalWithProduct(ValidatedProduct, categoryIds, ValidatedProduct.Discounts);
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
                products = await _internalProductManager.GetProductsForInternalUse();
                Categories = await _internalProductManager.GetCategories();
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }










        //Hilfsfunktionen - Auslagern in separate klasse, wie ist das dann mit dem IActionResult???

        public async Task<IActionResult> ReInitializeModalWithProduct(ValidatedProduct validatedProduct, List<Guid> categoryIds, List<DiscountDTO> discounts)
        {
            validatedProduct.SelectedCategoryIds = categoryIds;
            validatedProduct.Discounts = discounts;

            if (!validatedProduct.Discounts.IsNullOrEmpty())
            {
                validatedProduct.Discounts = validatedProduct.Discounts.OrderBy(d => d.StartDate).ToList();
            }

            await LoadDataAsync();

            //TODO: funktioniert nicht, Felder sind immer noch gefüllt
            //NewDiscount = new ValidatedDiscount { DiscountPercentage = 0, StartDate = null, EndDate = null};
            SelectedProductId = validatedProduct.ProductId;
            ValidatedProduct = validatedProduct;
            DiscountsJson = JsonSerializer.Serialize(validatedProduct.Discounts);

            ShowModal = true;

            return Page();
        }


        public async Task<IActionResult> ReInitializeModalAfterDiscountValidationError(string categoryIdsJson, List<CategoryDTO> categories, string validatedProductJson)
        {
            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            var productCategories = categories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId));

            (ValidatedProduct validatedProduct, IActionResult? validatedProductError) =
                await DeserializeWithTryCatchAsync<ValidatedProduct>(ValidatedProductJson, "Failed to parse ValidatedProduct.", "Discount couldn't be added.");
            if (validatedProductError != null) return validatedProductError;
            ValidatedProduct = validatedProduct;

            validatedProduct.SelectedCategoryIds = productCategories.Select(c => c.CategoryId).ToList();
            SelectedProductId = validatedProduct.ProductId;
            ValidatedProduct = validatedProduct;
            ValidatedProductJson = ValidatedProductJson;
            CategoryIdsJson = CategoryIdsJson;
            ShowModal = true;
            return Page();
        }


        public IActionResult ReInitializeAfterFailiedDiscountOverlapValidtion(List<CategoryDTO> selectedCategories, ValidatedProduct validatedProduct, string validatedProductJson, string categoryIdsJson)
        {
            {
                ValidatedProduct.SelectedCategoryIds = selectedCategories.Select(c => c.CategoryId).ToList();
                SelectedProductId = validatedProduct.ProductId;
                ValidatedProductJson = validatedProductJson;
                CategoryIdsJson = categoryIdsJson;
                ShowModal = true;
                DiscountOverlap = true;
                return Page();
            }
        }


        private bool IsModelStateValidForProduct(Guid productId)
        {
            var keysToValidate = productId == Guid.Empty
                ? new[] { "ValidatedProduct.Price", "ValidatedProduct.ProductId", "ValidatedProduct.Description", "ValidatedProduct.ProductName", "ValidatedProduct.AmountOnStock", /*"ValidatedProduct.ProductPicture",*/ "ValidatedProduct.SelectedCategoryIds" }
                : new[] {/* "DiscountsJson",*/ "ValidatedProduct.Price", "ValidatedProduct.ProductId", "ValidatedProduct.Description", "ValidatedProduct.ProductName", "ValidatedProduct.AmountOnStock", /*"ValidatedProduct.ProductPicture",*/ "ValidatedProduct.SelectedCategoryIds", "ValidatedProductJson", "CategoryIdsJson" };

            return keysToValidate.All(key => ModelState[key]?.ValidationState == ModelValidationState.Valid);
        }


        private bool IsModelStateIsInvalidForDiscount(ModelStateDictionary modelState)
        {
            return modelState["NewDiscount.EndDate"]?.ValidationState != ModelValidationState.Valid ||
                modelState["NewDiscount.StartDate"]?.ValidationState != ModelValidationState.Valid ||
                modelState["NewDiscount.DiscountPercentage"]?.ValidationState != ModelValidationState.Valid ||
                modelState["ValidatedProductJson"]?.ValidationState != ModelValidationState.Valid ||
                modelState["CategoryIdsJson"]?.ValidationState != ModelValidationState.Valid;
        }


        private async Task<(T? Result, IActionResult? Error)> DeserializeWithTryCatchAsync<T>(string json, string logError, string modelStateError)
        {
            try
            {
                var result = JsonSerializer.Deserialize<T>(json);
                return (result, null);
            }
            catch (JsonException ex)
            {
                _logger.LogError(logError);
                ModelState.AddModelError(string.Empty, modelStateError);
                ShowModal = true;
                await LoadDataAsync();
                return (default, Page());
            }
        }


        private async Task<IActionResult> ExecuteOnExceptionCatch(string logError, string modelStateError, Exception ex)
        {
            _logger.LogError(logError, ex);
            ModelState.AddModelError(string.Empty, modelStateError);
            ShowModal = true;
            await LoadDataAsync();
            return Page();
        }

        private ProductToStoreDTO CreateProductToStoreDTO(ValidatedProduct validatedProduct, List<CategoryDTO> categories)
        {
            return new ProductToStoreDTO
            {
                ProductName = validatedProduct.ProductName,
                ProductPicture = validatedProduct.ProductPicture,
                AmountOnStock = validatedProduct.AmountOnStock.Value,
                Description = validatedProduct.Description,
                Categories = categories,
                Discounts = validatedProduct.Discounts,
                Price = validatedProduct.Price.Value,
                ProductId = validatedProduct.ProductId,
            };
        }

        private ValidatedProduct CreateValidatedProduct(ProductForInternalUseDTO selectedProduct, List<Guid> categoryIds)
        {
            return new ValidatedProduct
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
        }



        // TODO Edit Discount
        //public IActionResult OnPostEditDiscount(Guid discountId)
        //{
        //    // hier muss ein Dialog zum Ändern geöffnet werden in der UI / sowas wie zur Eingabe, aber irgendwie unter dem Discount, man kann keine vergangenen Discounts ändern

        //    // Discount bearbeiten (Logik zur Anzeige der Edit-Werte)

        //    //Prüfen, ob Überlappung

        //    //Save Product incl CreateProductToStoreDTO(ValidatedProduct, selectedCategories)

        //    //neues Product mit allem Drum und dran laden.
        //    return Page();
        //}


    }
}