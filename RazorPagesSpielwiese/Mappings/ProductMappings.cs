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

        public Product ProductToStoreToProductEntity(ProductToStoreDTO productToStore, List<ProductCategoryMapping> categories, List<Discount> dicsounts)
        {
            var newProductId = productToStore.ProductId;
            if(newProductId == Guid.Empty)
            {
                newProductId = Guid.NewGuid();
            }
            return new Product
            {
                ProductId = newProductId,
                ProductName = productToStore.ProductName,
                ProductPicture = productToStore.ProductPicture,
                ProductCategoryMappings = categories,
                ProductDescription = productToStore.Description,
                ProductPrice = productToStore.Price,
                AmountOnStock = productToStore.AmountOnStock,
                Discounts = dicsounts,
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
