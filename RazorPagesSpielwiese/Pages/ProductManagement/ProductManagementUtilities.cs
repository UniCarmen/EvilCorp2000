﻿using EvilCorp2000.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using EvilCorp2000.Services;
using Microsoft.IdentityModel.Tokens;

namespace EvilCorp2000.Pages.ProductManagement
{
    public partial class ProductManagementModel
    {

        public async Task LoadDataAsync()
        {
            try
            {
                products = await _internalProductManager.GetProductsForInternalUse();
                Categories = await _internalProductManager.GetCategories();
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }


        public async Task<IActionResult> ReInitializeModalWithProduct(ValidatedProduct validatedProduct, List<Guid> categoryIds, List<DiscountDTO> discounts)
        {
            validatedProduct.SelectedCategoryIds = categoryIds;
            validatedProduct.Discounts = discounts;

            if (!validatedProduct.Discounts.IsNullOrEmpty())
            {
                validatedProduct.Discounts = validatedProduct.Discounts.OrderBy(d => d.StartDate).ToList();
            }

            await LoadDataAsync();

            //FEHLER: funktioniert nicht, Felder sind immer noch gefüllt
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


        private async Task<IActionResult> SaveProduct(ProductToStoreDTO newProduct, ValidatedProduct validatedProduct)
        {
            try
            {
                if (validatedProduct.ProductId == Guid.Empty)
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
                ModelState.AddModelError("ProductValidation", ex.Message);
                return await ReInitializeModalWithProduct(validatedProduct, validatedProduct.SelectedCategoryIds, validatedProduct.Discounts);
            }

            catch (Exception ex)
            {
                return await ExecuteOnExceptionCatch("Failed to save the product. {0}", "Failed to save the product.", ex);
            }

            ShowModal = false;
            return RedirectToPage();
        }


        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger, IWebHostEnvironment environment)
        {
            _internalProductManager = internalProductManager ?? throw new ArgumentNullException(nameof(internalProductManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment;
        }
    }
}