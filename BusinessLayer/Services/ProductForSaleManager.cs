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

        

        public async Task<ProductListReturn<ProductForSaleDTO>> GetProductsForSale(GetProductsParameters parameters) 
            //optionaler Parameter zur Sortierung / Filterung
        {
            parameters.PageNumber = (parameters.PageNumber.HasValue && parameters.PageNumber.Value > 0) ? parameters.PageNumber.Value : 1;
            parameters.PageSize = (parameters.PageSize.HasValue && parameters.PageSize.Value > 0) ? parameters.PageSize.Value : 10;
            

            var productListReturn = await _productRepository
            .GetAllProductsAsync(parameters);

            var productList = await GetCurrentDiscountsForProducts(productListReturn.ProductList);

            var productForSaleReturn = new ProductListReturn<ProductForSaleDTO>
            {
                ProductList = productList,
                ProductCount = productListReturn.ProductCount,
                MaxPageCount = productListReturn.MaxPageCount,
            };

            return productForSaleReturn;
            //return await GetCurrentDiscountsForProducts(productListReturn.ProductList);


            //TODO1: kann das weg??
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

        public async Task<List<ProductForSaleDTO>> GetHighlightedProducts()
        {
            var products = await _productRepository.GetHighlightProducts();

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
