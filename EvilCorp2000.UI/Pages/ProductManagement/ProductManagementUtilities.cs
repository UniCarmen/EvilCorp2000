using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Services;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using EvilCorp2000.UIModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using static EvilCorp2000.Pages.Utilities.Utilities;
using static Shared.Utilities.Utilities;
using DataAccess.Entities;

namespace EvilCorp2000.Pages.ProductManagement
{
    public partial class ProductManagementModel
    {


        public async Task LoadDataAsync(UIGetProductsParameters? parameters)
        {
            try
            {
                Categories = await _internalProductManager.GetCategories();

                PageNumber = (parameters.PageNumber.HasValue && parameters.PageNumber.Value > 0) ? parameters.PageNumber.Value : 1;
                PageSize = (parameters.PageSize.HasValue && parameters.PageSize.Value > 0) ? parameters.PageSize.Value : 10;
                Search = parameters.Search ?? "";
                if (!parameters.FilterCategoryString.IsNullOrEmpty() && !Categories.IsNullOrEmpty())
                {
                    FilterCategory =
                    Categories
                    .Find(c => c.CategoryId == Guid.Parse(parameters.FilterCategoryString)).CategoryId;
                    FilterCategoryString = parameters.FilterCategoryString;
                    //hier muss ich anhanf FilterCAtegoryString (GUID) die id (richtige Guid raussuchen)
                    //parameters.FilterCategory;
                }


                var getProductParameters = new GetProductsParameters()
                {
                    SortOrder = MapSortOrderString(parameters.SortOrderString),
                    PageNumber = PageNumber,
                    PageSize = PageSize,
                    CategoryId = FilterCategory,
                    Search = Search
                };

                var productListReturn = await _internalProductManager.GetProductsForInternalUse(getProductParameters);
                
                products = productListReturn.ProductList;
                CountProducts = productListReturn.ProductCount;
                MaxPageCount = productListReturn.MaxPageCount;
                Categories = await _internalProductManager.GetCategories();
                SortOrder = parameters.SortOrderString ?? "Default";
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }


        public async Task<IActionResult> ReInitializeModalWithProduct(ValidatedProduct validatedProduct, List<Guid> categoryIds, List<DiscountDTO> discounts, string? sortOrderString = null, int? pageNumber = 1, int? pageSize = 10)
        {
            validatedProduct.SelectedCategoryIds = categoryIds;
            validatedProduct.Discounts = discounts;

            if (!validatedProduct.Discounts.IsNullOrEmpty())
            {
                validatedProduct.Discounts = validatedProduct.Discounts.OrderBy(d => d.StartDate).ToList();
            }

            UIGetProductsParameters getProductsParams = new UIGetProductsParameters()
            {
                SortOrderString = sortOrderString,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            await LoadDataAsync(getProductsParams);

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

            //var productCategories = categories.FindAll(c => categoryIds.Exists(id => id == c.CategoryId));
            var productCategories = categories?.Where(c => categoryIds?.Contains(c.CategoryId) == true).ToList() ?? new List<CategoryDTO>();


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

        //protected virtual für Test
        protected virtual bool IsModelStateValidForProduct(Guid productId)
        {
            var keysToValidate = productId == Guid.Empty
                ? new[] { "ValidatedProduct.Price", "ValidatedProduct.ProductId", "ValidatedProduct.Description", "ValidatedProduct.ProductName", "ValidatedProduct.AmountOnStock", "ValidatedProduct.SelectedCategoryIds" }
                : new[] { "ValidatedProduct.Price", "ValidatedProduct.ProductId", "ValidatedProduct.Description", "ValidatedProduct.ProductName", "ValidatedProduct.AmountOnStock", "ValidatedProduct.SelectedCategoryIds", "ValidatedProductJson", "CategoryIdsJson" };

            foreach (var key in keysToValidate)
            {
                var state = ModelState[key]?.ValidationState;
                var a = key;
                var b = state;
                Debug.WriteLine($" {key}: {state}");
            }

            return keysToValidate.All(key => ModelState[key]?.ValidationState == ModelValidationState.Valid);
        }


        protected virtual bool IsModelStateIsInvalidForDiscount(ModelStateDictionary modelState)
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

                await LoadDataAsync(null);
                return (default, Page());
            }
        }


        //TODO 1: wenn die Fehler aus der BL / DAL kommen sollen sie dort geloggt werden und nicht in der UI!
        private async Task<IActionResult> ExecuteOnExceptionCatch(string logError, string modelStateError, Exception ex)
        {
            //_logger.LogError(logError, ex);
            ModelState.AddModelError(string.Empty, modelStateError);
            ShowModal = true;
            await LoadDataAsync(null);
            return Page();
        }


        private async Task<IActionResult> ExecuteOnDBExceptionCatch(string modelStateError, Exception ex)
        {
            ModelState.AddModelError(string.Empty, modelStateError);
            ShowModal = true;
            await LoadDataAsync(null);
            return Page();
        }

        private async Task<IActionResult> SaveProduct(ProductManagementProductDTO newProduct, ValidatedProduct validatedProduct)
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

            catch (DbUpdateException ex)
            {
                return await ExecuteOnDBExceptionCatch("Fehler in der Datenbank", ex);
            }

            catch (ValidationException ex)
            {
                var errorString = ex.Message.Split(";");

                var dictionary = errorString
                    .Select(item => item.Trim('[', ']')) 
                    .Select(item => item.Split(',', 2)) // Splitte in zwei Teile am ersten Komma -> theoretische Key, Value pairs
                    .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());

                if (dictionary.Keys.Contains("UniqueProductName"))
                {
                    var message = dictionary.GetValueOrDefault("UniqueProductName");
                    ModelState.AddModelError("UniqueProductName", message);
                }
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


        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger, IWebHostEnvironment environment, IAuthorizationService authorizationService)
        {
            _internalProductManager = internalProductManager ?? throw new ArgumentNullException(nameof(internalProductManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _environment = environment;
            _authorizationService = authorizationService;
        }
    }
}
