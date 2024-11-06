using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Repositories;

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

                var categories = product.ProductCategoryMappings
                    .Select(mapping => new CategoryDTO { CategoryName = mapping.Category.CategoryName, CategoryId = mapping.Category.CategoryId})
                    .ToList();

                var productMapper = new Mappings.ProductMappings();

                productsForInternalUse.Add(productMapper.ProductToProductForInternalUse(product, currentDiscounts, categories));
            }

            return productsForInternalUse;
        }

        //public async Task<ProductForSaleDTO> GetProductForInternalUse(int id)
        //{
        //    var productEntity = await _productRepository.GetProductById(id);

        //    if (productEntity == null)
        //    {
        //        throw new ArgumentNullException(nameof(productEntity));
        //    }

        //    var currentDiscounts = await _discoutRepository.GetDiscountsByProductId(id);

        //    var productMapper = new ProductMappings();

        //    return productMapper.ProductToProductForInternalUse(productEntity, currentDiscounts);
        //}

        public async Task<List<Models.CategoryDTO>> GetCategories()
        {
            //get all categories
            var categoryEntities = await _categoryRepository.GetAllCategories();

            //if (categoryEntities.Count == 0)
            //{ throw new ArgumentNullException(nameof(categoryEntities)); }

            var Mapping = new Mappings.CategoryMapping();
            return categoryEntities.Select(c => Mapping.CategoryEntityToCategoryModel(c)).ToList();
        }

        //TODO
        public async Task SaveProductToStore(ProductToStoreDTO productToStore)
        {
            var productMapper = new Mappings.ProductMappings();


            await _productRepository.AddProduct(productMapper.ProductToStoreToProductEntity(productToStore, [productToStore.Category]));
        }
    }
}
