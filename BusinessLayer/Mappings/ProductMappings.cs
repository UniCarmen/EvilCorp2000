using DataAccess.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Mappings
{
    public class ProductMappings
    {
        public ProductForSaleDTO ProductToProductForSale(Product productEntity, Discount currentDiscount)
        {
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

        public Product ProductManagementProductToProductEntity(ProductManagementProductDTO pmProduct, List<Category> categories, List<Discount> dicsounts)
        {
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
                Discounts = dicsounts,
                Rating = pmProduct.Rating
            };
        }


        public ProductManagementProductDTO ProductToProductManagementProduct(Product product, List<DiscountDTO> currentDiscounts, List<CategoryDTO> categories)
        {
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
