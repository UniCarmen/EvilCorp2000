using BusinessLayer.Models;
using EvilCorp2000.Pages.ProductManagement.Partials;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using SixLabors.ImageSharp;
using Microsoft.EntityFrameworkCore;
using FluentResults;
using static EvilCorp2000.Pages.Utilities.Utilities;
using System.Collections.Generic;
using EvilCorp2000.UIModels;
using Microsoft.AspNetCore.Authorization;

namespace EvilCorp2000.Pages.ProductManagement
{

    [Authorize(Roles = "CEOofDoom, Overseer, TaskDrone")]
    public partial class ProductManagementModel : PageModel
    {
        public bool ShowModal { get; set; } = false;

        //public bool ShowDeletionConfirmation { get; set; } = false;

        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IAuthorizationService _authorizationService;

        //TODO: Ich brauche ein UI Objekt, dass genauso aussieht, wie InternalProduct, aber nur in der UI verwendet wird -> UI Service oder so, der Mappings durchführt
        //Evtl auch für Category und DiscountDTO
        public List<ProductManagementProductDTO>? products;

        [BindProperty(SupportsGet = true)]
        public IFormFile? ImageFile { get; set; }

        public List<CategoryDTO>? Categories { get; set; }

        [BindProperty]
        public ValidatedDiscount? NewDiscount { get; set; }

        public Guid SelectedProductId { get; set; }
        public string ProductName { get; set; }

        [BindProperty]
        public ValidatedProduct ValidatedProduct { get; set; }

        [BindProperty]
        public string DiscountsJson { get; set; }

        [BindProperty]
        public bool DiscountOverlap { get; set; }

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


        public async Task OnGet()
        {
            try
            {
                await LoadDataAsync();
                //_logger.LogInformation("Test-Log: Dies ist ein Test für Logging.");
                //_logger.LogWarning("Test-Log: Warnung für Logging.");
                //_logger.LogError("Test-Log: Fehler für Logging.");
            }
            catch (DbUpdateException ex)
            {
                // Hier NICHT loggen, weil DAL das bereits gemacht hat
                ModelState.AddModelError("", "Es gab einen Fehler in der Datenbank.");
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }

        //public async Task<IActionResult> OnPostShowDeletionConfirmationModal(Guid productId, string productName)
        //{
        //    SelectedProductId = productId;

        //    ProductName = productName;

        //    await LoadDataAsync();

        //    ShowDeletionConfirmation = true;

        //    return Page();
        //}

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
            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
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

            return await SaveProduct(newProduct, ValidatedProduct);
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


            // find Product from DBContext to get all current Discounts
            (ValidatedProduct validatedProduct, IActionResult? validatedProductError) =
                await DeserializeWithTryCatchAsync<ValidatedProduct>(ValidatedProductJson, "Failed to parse ValidatedProduct.", "Discount couldn't be added.");
            if (validatedProductError != null) return validatedProductError;
            ValidatedProduct = validatedProduct;

            var newSelectedProduct = products.FirstOrDefault(p => p.ProductId == ValidatedProduct.ProductId);
            ValidatedProduct.Discounts = newSelectedProduct.Discounts;


            // get SelectedCategories to fill Fields after Reload of the Modal after Saving the Discount
            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            var selectedCategories = Categories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId));


            //add new Discount
            var newProduct = CreateProductToStoreDTO(ValidatedProduct, selectedCategories);

            return await AddDiscount(newDiscount, newProduct, validatedProduct, products, categoryIds);
        }


        private async Task<IActionResult>AddDiscount(DiscountDTO newDiscount, ProductManagementProductDTO newProduct, ValidatedProduct validatedProduct, List<ProductManagementProductDTO> products, List<Guid> categoryIds)
        {
            try
            {
                await _internalProductManager.AddDiscount(newDiscount, newProduct);
            }
            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError("DiscountValidation", ex.Message);
                return await ReInitializeModalWithProduct(validatedProduct, validatedProduct.SelectedCategoryIds, validatedProduct.Discounts);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be added.", ex);
            }


            // neues Product mit allen Discounts inkl. dem Neuem von DBContext holen (hat jetzt eine GUID)
            var selectedProduct = products.FirstOrDefault(p => p.ProductId == validatedProduct.ProductId);

            return await ReInitializeModalWithProduct(validatedProduct, categoryIds, selectedProduct.Discounts);
        }


        private async Task<IActionResult> ImageModelError(string key, string message, ValidatedProduct validatedProduct, List<Guid> categoryIds, List<DiscountDTO> discountDtos)
        {
            ModelState.AddModelError(key, message);
            return await ReInitializeModalWithProduct(validatedProduct, categoryIds, discountDtos);
        }
        

        private Result ImageValidations (IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return Result.Fail("Please select a valid image file.");
            }

            // Dateigrößenprüfung (z. B. maximal 2 MB)
            if (imageFile.Length > 2 * 1024 * 1024) // 2 MB
            {
                return Result.Fail("The file size exceeds the 2 MB limit.");
            }

            // MIME-Typ-Prüfung
            var allowedFileTypes = new[] { "image/jpeg", "image/png", "image/gif" };

            if (!allowedFileTypes.Contains(imageFile.ContentType))
            {
                return Result.Fail("Only JPEG, PNG, and GIF formats are allowed.");
            }

            // Dateierweiterung prüfen
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return Result.Fail("Invalid file extension. Only .jpg, .jpeg, .png, and .gif are allowed.");
            }

            // Bildinhalt validieren --> ins Backend? bzw. den Service??
            try
            {
                using var image = SixLabors.ImageSharp.Image.Load(imageFile.OpenReadStream());
            }
            catch
            {
                return Result.Fail("The uploaded file is not a valid image.");
            }
            return Result.Ok();
        }



        public async Task<IActionResult> OnPostImageUpload()
        {
            (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
            if (categoryIdsJsonError != null) return categoryIdsJsonError;

            (ValidatedProduct validatedProduct, IActionResult? validatedProductError) =
                await DeserializeWithTryCatchAsync<ValidatedProduct>(ValidatedProductJson, "Failed to parse ValidatedProduct.", "Discount couldn't be added.");
            if (validatedProductError != null) return validatedProductError;

            (List<DiscountDTO> deserializedDiscounts, IActionResult? discountDTOJsonError) =
                    await DeserializeWithTryCatchAsync<List<DiscountDTO>>(DiscountsJson, "Failed to parse DiscountDTO List.", "Product couldn't be added.");
            if (discountDTOJsonError != null) return discountDTOJsonError;


            //Validierung
            var validImageResult = ImageValidations(ImageFile);

            if (validImageResult.IsFailed)
            {
                var errors = validImageResult.Errors.ConvertAll(e => e.Message).Aggregate((a, b) => $"{a} {b}");
                return await ImageModelError((nameof(ImageFile)), errors, validatedProduct, categoryIds, deserializedDiscounts);
            }                           


            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);

            var savePath = Path.Combine(_environment.WebRootPath, "images", fileName);

            //Filestream öffnet eine neue Datei im Pfad
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                //kopiert den Inhalt des ImageFile in den FileStream
                await ImageFile.CopyToAsync(fileStream);
            }

            validatedProduct.ProductPicture = $"/images/{fileName}";

            #region
            //ALTE HERANSGEHENSWEISE, hat aber alles verlangsamt:
            //string productPictureToSave;

            //// Konvertiere das Bild in einen Base64-String
            //using (var memoryStream = new MemoryStream())
            //{
            //    await ImageFile.CopyToAsync(memoryStream);
            //    var imageBytes = memoryStream.ToArray();
            //    productPictureToSave = Convert.ToBase64String(imageBytes);
            //}

            //validatedProduct.ProductPicture = productPictureToSave;
            #endregion

            try
            {
                await _internalProductManager.SaveProductPicture(validatedProduct.ProductId, validatedProduct.ProductPicture);
            }
            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be added.", ex);
            }

            return await ReInitializeModalWithProduct(validatedProduct, categoryIds, deserializedDiscounts);
        }

        
        public async Task<IActionResult> OnPostDeleteProduct(Guid productId)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, "CanDeleteProducts");

            if (authResult == null || !authResult.Succeeded)
            {
                //return new ContentResult { Content = "ACCESS DENIED - 403", StatusCode = 403 };
                return RedirectToPage("/Error", new { code = 403 }); //Forbid(); 
            }


            try
            {
                await _internalProductManager.DeleteProduct(productId);
                ShowModal = false;
                return RedirectToPage();
            }
            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Failed to delete the product. {0}", "Failed to delete the product.", ex);
            }
        }


        public async Task<IActionResult> OnPostDeleteDiscount(Guid discountId, Guid productId) 
        {
            try
            {
                //Product neu laden, da es Probleme gibt, wenn ich mit dem alten ValidatedProduct weiterarbeiten würde
                var loadedProduct = await _internalProductManager.GetProductForInternalUse(productId);

                var selectedCategories = loadedProduct.Categories.Select(c => c.CategoryId).ToList();

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
                var newDisounts = ValidatedProduct.Discounts.FindAll(d => d.DiscountId != discountId);
                ValidatedProduct.Discounts = newDisounts;

                // Load SelectedCategories to fill Fields after Reload of the Modal after Saving the Discount
                (List<Guid> categoryIds, IActionResult? categoryIdsJsonError) =
                    await DeserializeWithTryCatchAsync<List<Guid>>(CategoryIdsJson, "Failed to parse CategoryIds.", "Discount couldn't be added.");
                if (categoryIdsJsonError != null) return categoryIdsJsonError;

                var productToStore = CreateProductToStoreDTO(ValidatedProduct, loadedProduct.Categories);

                //neues Product speichern
                await _internalProductManager.UpdateProductToStore(productToStore);

                return await ReInitializeModalWithProduct(ValidatedProduct, categoryIds, ValidatedProduct.Discounts);
            }
            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
            }
            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Error adding the discount.  {0}", "Discount couldn't be deleted.", ex);
            }
        }
    }
}