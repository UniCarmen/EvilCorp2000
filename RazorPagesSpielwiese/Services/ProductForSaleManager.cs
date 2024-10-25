using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Repositories;
using RazorPagesSpielwiese.Mappings;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
namespace RazorPagesSpielwiese.Services
{
    public class ProductForSaleManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;

        public ProductForSaleManager(IDiscountRepository discoutRepository, IProductRepository productRepository)
        {
            _discoutRepository = discoutRepository ?? throw new ArgumentNullException(nameof(discoutRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
        }

        public async Task<List<ProductForSaleDTO>> GetProductsForSale ()
        {
            var products = await _productRepository.GetAllProductsAsync();

            List<ProductForSaleDTO> productsForSale = [];

            foreach(Product product in products)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                var productMapper = new ProductMappings();

                productsForSale.Add(productMapper.ProductToProductForSale(product, currentDiscount));
            }
            
            return productsForSale;
        }

        public async Task<ProductForSaleDTO> GetProductForSale(int id)
        {
            var productEntity = await _productRepository.GetProductById(id);

            if (productEntity == null)
            {
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(id);

            var productMapper = new ProductMappings();

            return productMapper.ProductToProductForSale(productEntity, currentDiscount);
        }

        public static ProductForSaleManager Create ()
        {
            var dbContext = new EvilCorp2000ShopContext();
            return new ProductForSaleManager(new DiscountRepository(dbContext), new ProductRepository(dbContext));
        }
    }
}
