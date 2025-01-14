using EvilCorp2000.Entities;
using EvilCorp2000.Models;

namespace EvilCorp2000.Mappings
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


        //for update - brauche ich den Typen ProductToStore eigentlich noch??? veräderung für update direkt im repo
       public Product MapProductToStoreDTOToProductEntity(ProductToStoreDTO productToStore)
        {
            return new Product
            {
                ProductId = productToStore.ProductId,
                ProductName = productToStore.ProductName,
                ProductPicture = productToStore.ProductPicture,
                ProductDescription = productToStore.Description,
                ProductPrice = productToStore.Price,
                AmountOnStock = productToStore.AmountOnStock
            };
        }

        //Namen ändern?? in neues Product, oder andere Funktionen abändern, dass categorien und Listen separat verändert werden und dann obige Methode verwenden
        public Product ProductToStoreToProductEntity(ProductToStoreDTO productToStore, List<Category> categories, List<Discount> dicsounts)
        {
            var newProductId = productToStore.ProductId;
            if (newProductId == Guid.Empty)
            {
                newProductId = Guid.NewGuid();
            }
            return new Product
            {
                ProductId = newProductId,
                ProductName = productToStore.ProductName,
                ProductPicture = productToStore.ProductPicture,
                Categories = categories,
                ProductDescription = productToStore.Description,
                ProductPrice = productToStore.Price,
                AmountOnStock = productToStore.AmountOnStock,
                Discounts = dicsounts,
                Rating = productToStore.Rating
            };
        }


        public ProductForInternalUseDTO ProductToProductForInternalUse(Product product, List<DiscountDTO> currentDiscounts, List<CategoryDTO> categories)
        {
            return new ProductForInternalUseDTO
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

        //public Produ
    }
}
