using BusinessLayer.Mappings;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories;
using DataAccess.Entities;
using BusinessLayer.Models;
using Microsoft.Extensions.Logging;
using static Shared.Utilities.Utilities;
using Microsoft.Data.SqlClient;

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

        public async Task<List<ProductForSaleDTO>> GetProductsForSale(ProductSortOrder? sortOrder = null, int? pageNumber = 1, int? pageSize = 10) 
            //optionaler Parameter zur Sortierung / Filterung
        {
            var products = await _productRepository
            .GetAllProductsAsync(sortOrder, pageNumber, pageSize);

            return await GetCurrentDiscountsForProducts(products);

            //List<ProductForSaleDTO> productsForSale = [];

            //foreach (Product product in products)
            //{
            //    var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

            //    var productMapper = new ProductMappings();

            //    productsForSale.Add(productMapper.ProductToProductForSaleDto(product, currentDiscount));
            //}

            //return productsForSale;
        }

        public async Task<List<ProductForSaleDTO>> GetCurrentDiscountsForProducts (List<Product> productsWithAllDiscounts)
        {
            List<ProductForSaleDTO> productsForSale = [];

            foreach (Product product in productsWithAllDiscounts)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                var productMapper = new ProductMappings();

                productsForSale.Add(productMapper.ProductToProductForSaleDto(product, currentDiscount));
            }

            return productsForSale;
        }

        public async Task<List<ProductForSaleDTO>> GetHighlightProducts()
        {
            var products = await _productRepository
            .GetHighlightProducts();

            return await GetCurrentDiscountsForProducts(products);
        }


        public async Task<ProductForSaleDTO> GetProductForSale(Guid id)
        {
            var productEntity = await _productRepository.GetProductByIdWithCategoriesAnsdDiscounts(id);

            if (productEntity == null)
            {
                throw new ArgumentNullException(nameof(productEntity));
            }

            var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(id);

            var productMapper = new ProductMappings();

            return productMapper.ProductToProductForSaleDto(productEntity, currentDiscount);
        }
    }
}
