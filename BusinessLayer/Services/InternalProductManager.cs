using DataAccess.Entities;
using BusinessLayer.Models;
using DataAccess.Repositories;
using System.ComponentModel.DataAnnotations;
using BusinessLayer.Mappings;
using static BusinessLayer.Services.ValidationService;
using Shared.Utilities;
using System.Diagnostics.CodeAnalysis;
using static Shared.Utilities.Utilities;

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

        //TODO1: Methoden auslagern, vieles doppelt

        public async Task<ProductListReturn<ProductManagementProductDTO>> GetProductsForInternalUse(ProductSortOrder? sortOrderString = null)
        {
            var productListReturn = await _productRepository.GetAllProductsAsync(sortOrderString);

            List<ProductManagementProductDTO> productsForInternalUse = [];

            foreach (Product product in productListReturn.ProductList)
            {
                (List<DiscountDTO> currentDiscounts, List<CategoryDTO> categories) = 
                    MapDiscountsAndCategoriesToDTOs(product.Discounts.ToList(), product.Categories.ToList());
                
                productsForInternalUse.Add(_productMapper.ProductToProductManagementProductDto(product, currentDiscounts, categories));
            }

            var productManagementProductReturn = new ProductListReturn<ProductManagementProductDTO>
            {
                ProductList = productsForInternalUse,
                ProductCount = productListReturn.ProductCount,
                MaxPageCount = productListReturn.MaxPageCount,
            };

            return productManagementProductReturn;
        }


        public async Task<ProductManagementProductDTO> GetProductForInternalUse(Guid id)
        {
            var productEntity = Utilities.ReturnValueOrThrowExceptionWhenNull(
                await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(id), 
                "ProductEntity is null.");

            var currentDiscountEntities = await _discoutRepository.GetDiscountsByProductId(id);

            (List<DiscountDTO> currentDiscounts, List<CategoryDTO> categories) = 
                MapDiscountsAndCategoriesToDTOs(currentDiscountEntities, productEntity.Categories.ToList());

            return _productMapper.ProductToProductManagementProductDto(productEntity, currentDiscounts, categories);
        }

        public async Task<List<CategoryDTO>> GetCategories()
        {
            return
                ServiceUtilities.MapWithNullChecks(
                    await _categoryRepository.GetAllCategories(),
                    _categoryMapper.CategoryToCategoryDto,
                    "Category List")
                .ToList();
        }


        public async Task SaveProductToStore(ProductManagementProductDTO productToStore)
        {
            productToStore = Utilities.ReturnValueOrThrowExceptionWhenNull(productToStore, "ProductToStore is null.");

            var existingProduct = await _productRepository.GetProductById(productToStore.ProductId);

            if (existingProduct != null)
            {
                throw new ArgumentException("Product with this ID already exists.");
            }

            var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName, productToStore.ProductId);
            ValidateProduct(productToStore, nameIsUnique);

            (List<Discount> currentDiscounts, List<Category> categories) = 
                MapDiscountsAndCategoriesToEntities(productToStore.Discounts, productToStore.Categories, productToStore.ProductId);

            categories = _categoryRepository.AttachCategoriesIfNeeded(categories);

            await _productRepository.AddProduct(_productMapper.ProductManagementProductDtoToProductEntity(productToStore, categories, currentDiscounts));
          
        }


        public async Task AddDiscount(DiscountDTO discount, ProductManagementProductDTO productToStore)
        {
            discount = Utilities.ReturnValueOrThrowExceptionWhenNull(discount, "Discount is null.");
            productToStore.Discounts = Utilities.ReturnValueOrThrowExceptionWhenNull(productToStore.Discounts, "Discount is null.");

            ValidateDiscount(discount, productToStore.Discounts);

            discount.DiscountId = Guid.NewGuid();

            productToStore = Utilities.ReturnValueOrThrowExceptionWhenNull(productToStore, "productToStore is null");
            
            productToStore.Discounts.Add(discount);
            await UpdateProductToStore(productToStore);

        }

        public async Task UpdateProductToStore(ProductManagementProductDTO productToStore)
        {
            productToStore = Utilities.ReturnValueOrThrowExceptionWhenNull(productToStore, "productToStore is null.");

            var nameIsUnique = await _productRepository.IsProductNameUniqueAsync(productToStore.ProductName, productToStore.ProductId);
            ValidateProduct(productToStore, nameIsUnique);

            (List<Discount> currentDiscounts, List<Category> categories) =
                MapDiscountsAndCategoriesToEntities(productToStore.Discounts, productToStore.Categories, productToStore.ProductId);

            var productFromDB = Utilities.ReturnValueOrThrowExceptionWhenNull(
                await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(productToStore.ProductId),
                "productFromDB is null.");

            productFromDB.ProductName = productToStore.ProductName;
            productFromDB.ProductDescription = productToStore.Description;
            productFromDB.ProductPicture = productToStore.ProductPicture;
            productFromDB.ProductPrice = productToStore.Price;
            productFromDB.AmountOnStock = productToStore.AmountOnStock;
           
            await _productRepository.UpdateProduct(productFromDB);
            
            //TODO Update über Update Product funktioniert nicht. WaruM??? zu komplex???
            await _categoryRepository.UpdateCategories(productFromDB, categories);
            await _discoutRepository.UpdateDiscounts(productFromDB, currentDiscounts);
        }


        public async Task DeleteProduct (Guid productId)
        {
            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, "ProductId is null.");
            await _productRepository.DeleteProduct(productId);
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            var pictureStringInValid = string.IsNullOrEmpty(encodedPicture);
            if (productId == Guid.Empty || pictureStringInValid)
            { throw new ArgumentException(nameof(productId), "Guid is empty or pictureString is invalid"); }

            await _productRepository.SaveProductPicture(productId, encodedPicture);
        }

        public async Task DeleteProductPicture(Guid productId)
        {
            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, "ProductId is null.");
            await _productRepository.DeleteProductPicture(productId);
        }

        public (List<DiscountDTO>, List<CategoryDTO>) MapDiscountsAndCategoriesToDTOs(List<Discount> discounts, List<Category> categories)
        {
            List<DiscountDTO> returnDiscountsDtos =
                    ServiceUtilities.MapWithNullChecks(
                        discounts,
                        _discountMapper.DiscountToDiscountDTO,
                        "Discount List")
                    .ToList();

            List<CategoryDTO> returnCategorieDtos =
                ServiceUtilities.MapWithNullChecks(
                    categories,
                    _categoryMapper.CategoryToCategoryDto,
                    "Category List")
                .ToList();

            return (returnDiscountsDtos, returnCategorieDtos);
        }


        public (List<Discount>, List<Category>) MapDiscountsAndCategoriesToEntities(List<DiscountDTO> discounts, List<CategoryDTO> categories, Guid productId)
        {
            List<Discount> returnDiscounts =
                    ServiceUtilities.MapWithGuidWithNullChecks(
                        discounts,
                        _discountMapper.DiscountDTOToDiscount,
                        productId,
                        "Discount List")
                    .ToList();

            List<Category> returnCategories =
                ServiceUtilities.MapWithNullChecks(
                    categories,
                    _categoryMapper.CategoryDtoToCategory,
                    "Category List")
                .ToList();

            return (returnDiscounts, returnCategories);
        }
    }
}
