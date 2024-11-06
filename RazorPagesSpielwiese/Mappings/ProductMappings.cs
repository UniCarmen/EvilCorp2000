using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Mappings
{
    public class ProductMappings
    {
        public ProductForSaleDTO ProductToProductForSale (Product productEntity, Discount currentDiscount, List<string> categories)
        {
            decimal discountedPrice = 0.0m;
            double discountedPercentage;

            if (currentDiscount != null && currentDiscount.DiscountPercentage != 0.0)
            { 
                discountedPrice = (decimal)(((double)productEntity.ProductPrice) / 100 * (100 - currentDiscount.DiscountPercentage));
                discountedPercentage = currentDiscount.DiscountPercentage;
            }
            else
            { 
                discountedPrice = 0.0m;
                discountedPercentage = 0.0;
            }

            return new ProductForSaleDTO
            {
                ProductId = productEntity.ProductId,
                ProductName = productEntity.ProductName,
                ProductPicture = productEntity.ProductPicture,
                AmountOnStock = productEntity.AmountOnStock,
                Categories = categories,
                Description = productEntity.ProductDescription ?? "",
                Discount = discountedPercentage,
                DiscountedPrice = discountedPrice,
                Price = productEntity.ProductPrice,
                Rating = productEntity.Rating
            };
        }

        public Product ProductToStoreToProductEntity(ProductToStoreDTO productToStore, List<Models.CategoryDTO> categories)
        {
            var newProductId = Guid.NewGuid();
            var productCategoryMapping = categories.Select(c => new ProductCategoryMapping { ProductId = newProductId, CategoryId = c.CategoryId });
            return new Product
            {
                ProductId = newProductId,
                ProductName = productToStore.ProductName,
                ProductPicture = productToStore.ProductPicture,
                ProductCategoryMappings = productCategoryMapping.ToList(),
                ProductDescription = productToStore.Description,
                ProductPrice = productToStore.Price,
                //noch kein Discount und Rating
            };
        }


        public ProductForInternalUseDTO ProductToProductForInternalUse(Product product, List<DiscountDTO> currentDiscounts, List<CategoryDTO>categories)
        {
            return new ProductForInternalUseDTO { 
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
