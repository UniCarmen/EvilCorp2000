using DataAccess.Entities;
using BusinessLayer.Models;
using Microsoft.IdentityModel.Tokens;
using Shared.Utilities;

namespace BusinessLayer.Mappings
{
    public class ProductMappings
    {
        public ProductForSaleDTO ProductToProductForSaleDto(Product productEntity, Discount currentDiscount)
        {
            productEntity = Utilities.ReturnValueOrThrowExceptionWhenNull(productEntity, "Category is null.");

            decimal? discountedPrice ;
            double discountedPercentage;

            //INFO: ?: Null Conditional Operator. wenn currentDiscount = null, dann wird DiscountPercentage gar nicht erst aufgerufen
            //INFO: ??: Null Coalescing Operator. wenn null wird 0.0 genommen
            discountedPercentage = currentDiscount?.DiscountPercentage ?? 0.0;

            //INFO: ?: -> ternärer Operator (=if ? = true, dann das : false, dann das)
            discountedPrice = discountedPercentage > 0 ?
                productEntity.ProductPrice * (decimal)((100 - discountedPercentage) / 100)
                : null;

            //INFO: ersetzt:
            //if (currentDiscount != null && currentDiscount.DiscountPercentage != 0.0)
            //{
            //    discountedPrice = (decimal)((double)productEntity.ProductPrice / 100 * (100 - currentDiscount.DiscountPercentage));
            //    discountedPercentage = currentDiscount.DiscountPercentage;
            //}
            //else
            //{
            //    discountedPrice = 0.0m;
            //    discountedPercentage = 0.0;
            //}

            return new ProductForSaleDTO
            {
                ProductId = productEntity.ProductId,
                ProductName = productEntity.ProductName,
                ProductPicture = productEntity.ProductPicture,
                AmountOnStock = productEntity.AmountOnStock,
                Categories = productEntity.Categories.Select(c => c.CategoryName).ToList(),
                Description = productEntity.ProductDescription ?? "",
                Discount = discountedPercentage,
                DiscountedPrice = discountedPrice,
                Price = productEntity.ProductPrice,
                Rating = productEntity.Rating
            };
        }

        public Product ProductManagementProductDtoToProductEntity(ProductManagementProductDTO pmProduct, List<Category> categories, List<Discount> discounts)
        {
            pmProduct = Utilities.ReturnValueOrThrowExceptionWhenNull(pmProduct, "ProductManagementProduct is null.");

            categories = categories.IsNullOrEmpty() ? throw new ArgumentException(nameof(categories), "Categories are null or empty.") : categories;
            
            var newProductId = pmProduct.ProductId == Guid.Empty ? Guid.NewGuid() : pmProduct.ProductId;

            return new Product
            {
                ProductId = newProductId,
                ProductName = pmProduct.ProductName,
                ProductPicture = pmProduct.ProductPicture,
                Categories = categories,
                ProductDescription = pmProduct.Description,
                ProductPrice = pmProduct.Price,
                AmountOnStock = pmProduct.AmountOnStock,
                Discounts = discounts,
                Rating = pmProduct.Rating
            };
        }


        public ProductManagementProductDTO ProductToProductManagementProductDto(Product product, List<DiscountDTO> currentDiscounts, List<CategoryDTO> categories)
        {
            product = Utilities.ReturnValueOrThrowExceptionWhenNull(product, "Product is null");

            categories = categories.IsNullOrEmpty() ? throw new ArgumentException(nameof(categories), "Category is null or empty") : categories;

            return new ProductManagementProductDTO
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPicture = product.ProductPicture,
                Price = product.ProductPrice,
                AmountOnStock = product.AmountOnStock,
                Categories = categories,
                Description = product.ProductDescription ?? "",
                Discounts = currentDiscounts,
                Rating = product.Rating
            };
        }

    }
}
