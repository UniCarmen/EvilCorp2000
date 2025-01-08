using Microsoft.EntityFrameworkCore;
using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Repositories;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace RazorPagesSpielwiese.Services
{
    public class InternalProductManager : IInternalProductManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public InternalProductManager(IDiscountRepository discoutRepository, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _discoutRepository = discoutRepository ?? throw new ArgumentNullException(nameof(discoutRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<List<ProductForInternalUseDTO>> GetProductsForInternalUse()
        {
            var products = await _productRepository.GetAllProductsAsync();

            List<ProductForInternalUseDTO> productsForInternalUse = [];

            foreach (Product product in products)
            {
                var currentDiscountEntities = await _discoutRepository.GetDiscountsByProductId(product.ProductId);

                var discountMapper = new Mappings.DiscountMappings();
                var currentDiscounts = currentDiscountEntities.Select(de => discountMapper.DiscountToDiscountDTO(de)).ToList();

                var categorieMapper = new Mappings.CategoryMappings();
                var categories = product.Categories.Select(c => categorieMapper.CategoryEntityToCategoryModel(c)).ToList();

                var productMapper = new Mappings.ProductMappings();

                productsForInternalUse.Add(productMapper.ProductToProductForInternalUse(product, currentDiscounts, categories));
            }

            return productsForInternalUse;
        }

        public async Task<ProductForInternalUseDTO> GetProductForInternalUse(Guid id)
        {
            var productEntity = await _productRepository.GetProductById(id);

            if (productEntity == null)
            {
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscountEntities = await _discoutRepository.GetDiscountsByProductId(id);

            var discountMapper = new Mappings.DiscountMappings();
            var currentDiscounts = currentDiscountEntities.Select(de => discountMapper.DiscountToDiscountDTO(de)).ToList();

            var categorieMapper = new Mappings.CategoryMappings();
            var categories = productEntity.Categories.Select(c => categorieMapper.CategoryEntityToCategoryModel(c)).ToList();

            var productMapper = new Mappings.ProductMappings();

            return productMapper.ProductToProductForInternalUse(productEntity, currentDiscounts, categories);
        }

        public async Task<List<Models.CategoryDTO>> GetCategories()
        {
            var categoryEntities = await _categoryRepository.GetAllCategories();

            var Mapping = new Mappings.CategoryMappings();
            return categoryEntities.Select(c => Mapping.CategoryEntityToCategoryModel(c)).ToList();
        }

        //TODO
        public async Task SaveProductToStore(ProductToStoreDTO productToStore)
        {
            if (productToStore != null)
            {
                var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName);
                ValidateProduct(productToStore, nameIsUnique);

                var productMapper = new Mappings.ProductMappings();
                var discountMapper = new Mappings.DiscountMappings();
                var categoryMapper = new Mappings.CategoryMappings();

                var discounts = productToStore.Discounts.Select(d => discountMapper.DiscountDTOToDiscount(d, productToStore.ProductId)).ToList();

                var categories = productToStore.Categories.Select(c =>
                    categoryMapper.CategoryDtoToCategory(c)
                ).ToList();

                categories = _categoryRepository.AttachCategoriesIfNeeded(categories);

                await _productRepository.AddProduct(productMapper.ProductToStoreToProductEntity(productToStore, categories, discounts));
            }
            else
            {
                throw new ArgumentNullException(nameof(productToStore));
            }
        }


        public async Task AddDiscount(DiscountDTO discount, ProductToStoreDTO productToStore)
        {
            if (discount == null)
            {
                throw new ArgumentNullException(nameof(discount));
            }

            var existingDiscounts = await _discoutRepository.GetDiscountsByProductId(productToStore.ProductId);
            ValidateDiscountAsync(discount, existingDiscounts);


            var discountMapper = new Mappings.DiscountMappings();
            var newDiscount = discountMapper.SetDiscountId(discount);

            if (productToStore == null)
                throw new ArgumentNullException(nameof(productToStore));

            productToStore.Discounts.Add(newDiscount);
            await UpdateProductToStore(productToStore);
            
        }

        public async Task UpdateProductToStore(ProductToStoreDTO productToStore)
        {
            if (productToStore != null)
            {
                var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName);
                ValidateProduct(productToStore, nameIsUnique);

                var productMapper = new Mappings.ProductMappings();
                var discountMapper = new Mappings.DiscountMappings();
                var categoryMapper = new Mappings.CategoryMappings();

                var discounts = productToStore.Discounts.Select(d => discountMapper.DiscountDTOToDiscount(d, productToStore.ProductId)).ToList();

                var categories = productToStore.Categories.Select(c => categoryMapper.CategoryDtoToCategory(c)).ToList();

                //markiert Die Kategorien im Context in den Categories als bereits existierend, damit nicht versucht wird, diese neu anzulegen
                categories = _categoryRepository.AttachCategoriesIfNeeded(categories);

                await _productRepository.UpdateProduct(productMapper.ProductToStoreToProductEntity(productToStore, categories, discounts));
            }
            else
            {
                throw new ArgumentNullException(nameof(productToStore));
            }
        }





        public void ValidateProduct(ProductToStoreDTO productToStore, bool nameIsUnique)
        {
            var validationErrors = new List<string>();

            // Verschiedene Validierungen prüfen und Fehler sammeln
            if (!nameIsUnique)
            {
                validationErrors.Add("Product name must be unique.");
            }

            if (productToStore.Price <= 0.0m)
            {
                validationErrors.Add("Price must be greater than 0.");
            }

            if (productToStore.AmountOnStock < 0)
            {
                validationErrors.Add("Amount on stock cannot be negative.");
            }

            // Wenn Fehler vorhanden sind, alle weitergeben
            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(" ", validationErrors));
            }
        }


        public void ValidateDiscountAsync(DiscountDTO discount, List<Discount> discounts)
        {
            var validationErrors = new List<string>();

            if (discount.StartDate < DateTime.Today || discount.EndDate < DateTime.Today)
                validationErrors.Add("Start and End dates must not be in the past.");

            if (discount.StartDate >= discount.EndDate)
                validationErrors.Add("End Date must be after Start Date.");

            if (discount.DiscountPercentage <= 0)
                validationErrors.Add("Discount Percentage must be greater than 0.");

            if (discounts.Any(d => discount.StartDate < d.EndDate && discount.EndDate > d.StartDate))
            {
                validationErrors.Add("Discount overlaps with an existing discount.");
            }

            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(" ", validationErrors));
            }
        }
    }
}
