﻿using EvilCorp2000.Entities;
using EvilCorp2000.Models;
using EvilCorp2000.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace EvilCorp2000.Services
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

        public async Task<List<CategoryDTO>> GetCategories()
        {
            var categoryEntities = await _categoryRepository.GetAllCategories();

            var Mapping = new Mappings.CategoryMappings();
            return categoryEntities.Select(c => Mapping.CategoryEntityToCategoryModel(c)).ToList();
        }

        //TODO Save NEW Product
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

            //var existingDiscounts = await _discoutRepository.GetDiscountsByProductId(productToStore.ProductId);
            ValidateDiscountAsync(discount, productToStore.Discounts);


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
                //var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName);
                //ValidateProduct(productToStore, nameIsUnique);

                var productMapper = new Mappings.ProductMappings();
                var discountMapper = new Mappings.DiscountMappings();
                var categoryMapper = new Mappings.CategoryMappings();

                var discounts = productToStore.Discounts.Select(d => discountMapper.DiscountDTOToDiscount(d, productToStore.ProductId)).ToList();

                var categories = productToStore.Categories.Select(c => categoryMapper.CategoryDtoToCategory(c)).ToList();

                //noch nötig? ich update die categorien ja jetzt separat vom Product
                //markiert Die Kategorien im Context in den Categories als bereits existierend, damit nicht versucht wird, diese neu anzulegen
                //categories = _categoryRepository.AttachCategoriesIfNeeded(categories);

                var productFromDB = await _productRepository.GetProductById(productToStore.ProductId);

                if (productFromDB == null)
                {
                    { throw new InvalidOperationException(nameof(productFromDB)); };
                }

                //das product hat weder categories noch discounts
                var newProductEntity = productMapper.MapProductToStoreDTOToProductEntity(productToStore);
                //newProductEntity.Discounts = productFromDB.Discounts;

                await _productRepository.UpdateProduct(newProductEntity, productFromDB);

                await _categoryRepository.UpdateCategories(productFromDB, categories);

                await _discoutRepository.UpdateDiscounts(productFromDB, discounts);
            }
            else
            {
                throw new ArgumentNullException(nameof(productToStore));
            }
        }





        public void ValidateProduct(ProductToStoreDTO productToStore, bool nameIsUnique)
        {
            var validationErrors = new List<string>();

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

            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(" ", validationErrors));
            }
        }


        public void ValidateDiscountAsync(DiscountDTO discount, List<DiscountDTO> discounts)
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


        //public async void DeleteProductAndItsDiscounts (Guid productId)
        //{
        //    await _productRepository.DeleteProduct(product);
        //}
    }
}
