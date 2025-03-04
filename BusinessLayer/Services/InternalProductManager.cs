using DataAccess.Entities;
using BusinessLayer.Models;
using DataAccess.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Mappings;
using static BusinessLayer.Services.ValidationService;

namespace BusinessLayer.Services
{
    public class InternalProductManager : IInternalProductManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ProductMappings _productMapper = new();
        private readonly DiscountMappings _discountMapper = new();
        private readonly CategoryMappings _categoryMapper = new();

        public InternalProductManager(
            IDiscountRepository discountRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository)
        {
            _discoutRepository = discountRepository ?? throw new ArgumentNullException(nameof(discountRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        //TODO: Nullprüfungen vor den Mappings

        public async Task<List<ProductManagementProductDTO>> GetProductsForInternalUse()
        {
            var products = await _productRepository.GetAllProductsAsync();

            List<ProductManagementProductDTO> productsForInternalUse = [];

            //TODO1: LINQ!
            foreach (Product product in products)
            {
                var currentDiscounts = product.Discounts.Select(de => _discountMapper.DiscountToDiscountDTO(de)).ToList();

                var categories = product.Categories.Select(c => _categoryMapper.CategoryToCategoryDto(c)).ToList();

                productsForInternalUse.Add(_productMapper.ProductToProductManagementProductDto(product, currentDiscounts, categories));
            }

            return productsForInternalUse;
        }

        public async Task<ProductManagementProductDTO> GetProductForInternalUse(Guid id)
        {
            var productEntity = await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(id);
            //TODO1: KeyNotFoundException, da ein Null-Product ein loguscher Fehler ist, kein fehlerhafter Parameter - noch woanders?
            //TODO1: direkt bei productEntity mit ?? throw new, damit der Fehler gleich abgefangen wird

            if (productEntity == null)
            {
                
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscountEntities = await _discoutRepository.GetDiscountsByProductId(id);

            var currentDiscounts = currentDiscountEntities.Select(de => _discountMapper.DiscountToDiscountDTO(de)).ToList();

            var categories = productEntity.Categories.Select(c => _categoryMapper.CategoryToCategoryDto(c)).ToList();

            return _productMapper.ProductToProductManagementProductDto(productEntity, currentDiscounts, categories);
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            //var categoryEntities = await _categoryRepository.GetAllCategories();
            var categoryEntities = await _categoryRepository.GetAllCategories();

            return categoryEntities.Select(c => _categoryMapper.CategoryToCategoryDto(c)).ToList();
        }


        public async Task SaveProductToStore(ProductManagementProductDTO productToStore)
        {
            if (productToStore == null)
            {
                throw new ArgumentNullException(nameof(productToStore));
            }

            var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName, productToStore.ProductId);
            ValidateProduct(productToStore, nameIsUnique);

            var discounts = productToStore.Discounts.Select(d => _discountMapper.DiscountDTOToDiscount(d, productToStore.ProductId)).ToList();

            var categories = productToStore.Categories.Select(c =>
                _categoryMapper.CategoryDtoToCategory(c)
            ).ToList();

            categories = _categoryRepository.AttachCategoriesIfNeeded(categories);

            await _productRepository.AddProduct(_productMapper.ProductManagementProductDtoToProductEntity(productToStore, categories, discounts));
          
        }


        public async Task AddDiscount(DiscountDTO discount, ProductManagementProductDTO productToStore)
        {
            if (discount == null)
            {
                throw new ArgumentNullException(nameof(discount));
            }

            ValidateDiscount(discount, productToStore.Discounts);

            discount.DiscountId = Guid.NewGuid();

            if (productToStore == null)
            {
                throw new ArgumentNullException(nameof(productToStore));
            }
            
            productToStore.Discounts.Add(discount);
            await UpdateProductToStore(productToStore);

        }

        public async Task UpdateProductToStore(ProductManagementProductDTO productToStore)
        {
            if (productToStore == null)
            {
                throw new ArgumentNullException(nameof(productToStore));
            }

            var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName, productToStore.ProductId);
            ValidateProduct(productToStore, nameIsUnique);

            var discounts = productToStore.Discounts.Select(d => _discountMapper.DiscountDTOToDiscount(d, productToStore.ProductId)).ToList();

            var categories = productToStore.Categories.Select(c => _categoryMapper.CategoryDtoToCategory(c)).ToList();

            var productFromDB = await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(productToStore.ProductId);

            if (productFromDB == null)
            {
                throw new InvalidOperationException(nameof(productFromDB));
            }

            productFromDB.ProductName = productToStore.ProductName;
            productFromDB.ProductDescription = productToStore.Description;
            productFromDB.ProductPicture = productToStore.ProductPicture;
            productFromDB.ProductPrice = productToStore.Price;
            productFromDB.AmountOnStock = productToStore.AmountOnStock;

            //TODO 1: Warum mache ich das so? -> ein neues Objekt erstellen und damit das DB product updaten?
            //var newProductEntity = (_productMapper.ProductManagementProductToProductEntity(productToStore, categories, discounts));
            await _productRepository.UpdateProduct(/*newProductEntity,*/ productFromDB);
            
            //Update über Update Product funktioniert nicht??
            await _categoryRepository.UpdateCategories(productFromDB, categories);
            await _discoutRepository.UpdateDiscounts(productFromDB, discounts);
        }


        public async Task DeleteProduct (Guid productId)
        {
            if (productId == Guid.Empty)
            { throw new ArgumentException("Ungültige Produkt-ID.", nameof(productId)); }
            await _productRepository.DeleteProduct(productId);
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            var pictureStringInValid = string.IsNullOrEmpty(encodedPicture);
            if (productId == Guid.Empty || pictureStringInValid)
            { throw new ArgumentNullException(nameof(productId)); }

            await _productRepository.SaveProductPicture(productId, encodedPicture);
        }

        public async Task DeleteProductPicture(Guid productId)
        {
            if (productId == Guid.Empty)
            { throw new ArgumentNullException(nameof(productId)); }

            await _productRepository.DeleteProductPicture(productId);
        }
    }
}
