using BusinessLayer.Mappings;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories;
using DataAccess.Entities;
using BusinessLayer.Models;
using Microsoft.Extensions.Logging;

namespace BusinessLayer.Services
{
    //wenn der Shop geschrieben wird, noch einmal komplett prüfen / überarbeiten
    //TODO: muss umgeschrieben werden, sodass mit InternalProduct gearbeitet wird, in der UI muss dann zu ProductForSale gemappt werden 
    //Klasse üerprüfen -> geplante Änderungen, try catches mit
    //sollte Fehler in die DB schreiben
    // _logger.LogError(ex, "Fehler beim Verarbeiten von Produkt: {ProductName}", productName);
    public class ProductForSaleManager : IProductForSaleManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;
        //private readonly ICategoryRepository _categoryRepository;

        public ProductForSaleManager(IDiscountRepository discoutRepository, IProductRepository productRepository/*, ICategoryRepository categoryRepository*/)
        {
            _discoutRepository = discoutRepository ?? throw new ArgumentNullException(nameof(discoutRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            //_categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<List<ProductForSaleDTO>> GetProductsForSale()
        {
            var products = await _productRepository
            .GetAllProductsAsync();

            List<ProductForSaleDTO> productsForSale = [];

            foreach (Product product in products)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                var productMapper = new ProductMappings();

                productsForSale.Add(productMapper.ProductToProductForSaleDto(product, currentDiscount));
            }

            return productsForSale;
        }

        public async Task<ProductForSaleDTO> GetProductForSale(Guid id)
        {
            var productEntity = await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(id);

            if (productEntity == null)
            {
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(id);

            //var categories = productEntity.ProductCategoryMappings
            //        .Select(mapping => mapping.Category.CategoryName)
            //        .ToList();

            var productMapper = new Mappings.ProductMappings();

            return productMapper.ProductToProductForSaleDto(productEntity, currentDiscount);
        }
    }
}
