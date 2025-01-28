using BusinessLayer.Mappings;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore;
using DataAccess.Repositories;
using DataAccess.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Services
{
    //TODO: muss umgeschrieben werden, sodass mit InternalProduct gearbeitet wird, in der UI muss dann zu ProductForSale gemappt werden 
    public class ProductForSaleManager : IProductForSaleManager
    {
        private readonly IDiscountRepository _discoutRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductForSaleManager(IDiscountRepository discoutRepository, IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _discoutRepository = discoutRepository ?? throw new ArgumentNullException(nameof(discoutRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
        }

        public async Task<List<ProductForSaleDTO>> GetProductsForSale()
        {
            var products = await _productRepository
            .GetAllProductsAsync();

            //categirien abrufen
            //mit jointabelle abgleichen, welche Kategorie zu welchem mprodukgt

            List<ProductForSaleDTO> productsForSale = [];

            foreach (Product product in products)
            {
                var currentDiscount = await _discoutRepository.GetCurrentDiscountByProductId(product.ProductId);

                //var categories = product.ProductCategoryMappings
                //    .Select(mapping => mapping.Category.CategoryName)
                //    .ToList();

                //if (categories.Count == 0)
                //{
                //    throw new ArgumentNullException(nameof(categories));
                //}

                var productMapper = new Mappings.ProductMappings();

                productsForSale.Add(productMapper.ProductToProductForSale(product, currentDiscount));
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

            return productMapper.ProductToProductForSale(productEntity, currentDiscount);
        }
    }
}
