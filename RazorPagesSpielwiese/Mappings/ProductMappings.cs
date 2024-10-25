using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Mappings
{
    public class ProductMappings
    {
        public ProductForSaleDTO ProductToProductForSale (Product productEntity, Discount currentDiscount)
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
                Category = productEntity.ProductClass,
                Description = productEntity.ProductDescription,
                Discount = discountedPercentage,
                DiscountedPrice = discountedPrice,
                Price = productEntity.ProductPrice,
                Rating = productEntity.Rating
            };
        }
    }
}
