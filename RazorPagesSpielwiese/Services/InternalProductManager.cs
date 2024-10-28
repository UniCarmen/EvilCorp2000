using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Mappings;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Repositories;

namespace RazorPagesSpielwiese.Services
{
    public class InternalProductManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;

        public InternalProductManager(IDiscountRepository discoutRepository, IProductRepository productRepository)
        {
            _discoutRepository = discoutRepository ?? throw new ArgumentNullException(nameof(discoutRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<List<ProductForSaleDTO>> GetProductsForInternalUse()
        {
            var products = await _productRepository.GetAllProductsAsync();

            List<ProductForSaleDTO> productsForInternalUse = [];

            foreach (Product product in products)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                var productMapper = new ProductMappings();

                productsForInternalUse.Add(productMapper.ProductToProductForInternalUse(product, currentDiscount));
            }

            return productsForInternalUse;
        }

        public async Task<ProductForSaleDTO> GetProductForInternalUse(int id)
        {
            var productEntity = await _productRepository.GetProductById(id);

            if (productEntity == null)
            {
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscounts = await _discoutRepository.GetDiscountsByProductId(id);

            var productMapper = new ProductMappings();

            return productMapper.ProductToProductForInternalUse(productEntity, currentDiscounts);
        }

        //public static ProductForSaleManager Create()
        //{
        //    var dbContext = new EvilCorp2000ShopContext();
        //    return new ProductForSaleManager(new DiscountRepository(dbContext), new ProductRepository(dbContext));
        //}

        public async Task<List<ProductForSaleDTO>> GetCategories()
        {
            //get all categories
            var products = await _productRepository.GetAllProductsAsync();

            List<ProductForSaleDTO> productsForSale = [];

            foreach (Product product in products)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                var productMapper = new ProductMappings();

                productsForSale.Add(productMapper.ProductToProductForInternalUse(product, currentDiscount));
            }

            return productsForSale;
        }

        public async Task SaveProductToStore (ProductToStoreDTO)
        {
            var productMapper = new ProductMappings();

            await _productRepository.AddProduct(productMapper.ProductToProductForInternalUse(product, currentDiscount));
        }

    }
}
