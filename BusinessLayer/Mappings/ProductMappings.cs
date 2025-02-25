using DataAccess.Entities;
using BusinessLayer.Models;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLayer.Mappings
{
    public class ProductMappings
    {
        public ProductForSaleDTO ProductToProductForSaleDto(Product productEntity, Discount currentDiscount)
        {
            if (productEntity == null)
            { throw new ArgumentNullException (nameof(productEntity)); }

            decimal discountedPrice = 0.0m;
            double discountedPercentage;

            if (currentDiscount != null && currentDiscount.DiscountPercentage != 0.0)
            {
                discountedPrice = (decimal)((double)productEntity.ProductPrice / 100 * (100 - currentDiscount.DiscountPercentage));
                discountedPercentage = currentDiscount.DiscountPercentage;
            }
            else
            {
                discountedPrice = 0.0m;
                discountedPercentage = 0.0;
            }

            //TODO1: Besser
            //INFO: ?: Null Conditional Operator. wenn currentDiscount = 0, dann wird DiscountPercentage nicht aufgerufen
            //INFO: ??: Null Coalescing Operator. Wenn currentDiscount = 0, dann Standardwerd 0.0
            //INFO: Ansonsten discountPercentage
            //double discount = currentDiscount?.DiscountPercentage ?? 0.0;

            //INFO: ?: -> ternärer Operator (=if ?=true, :false)
            //decimal discountedPrice = discount > 0
            //    ? productEntity.ProductPrice * (decimal)((100 - discount) / 100)
            //    : 0.0m;

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
            if (pmProduct == null)
            { throw new ArgumentNullException(nameof(pmProduct)); }

            if(categories.IsNullOrEmpty())
            {  throw new ArgumentException(nameof(categories)); }
            
            var newProductId = pmProduct.ProductId;
            if (newProductId == Guid.Empty)
            {
                newProductId = Guid.NewGuid();
            }
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
            if (product == null)
            { throw new ArgumentNullException(nameof(product)); }

            if (categories.IsNullOrEmpty())
            { throw new ArgumentException(nameof(categories)); }

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
