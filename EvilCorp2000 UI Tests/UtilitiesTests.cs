using BusinessLayer.Models;
using EvilCorp2000.Pages.Utilities;
using EvilCorp2000.UIModels;
using System;
using System.Collections.Generic;
using Xunit;

namespace EvilCorp2000_UI_Tests
{
    public class UtilitiesTests
    {
        [Fact]
        public void CreateValidatedProduct_ShouldMapProperties()
        {
            // ARRANGE
            var discountId = Guid.NewGuid();
            var productDto = new ProductManagementProductDTO
            {
                ProductId = Guid.NewGuid(),
                ProductPicture = "picture.png",
                ProductName = "Test Product",
                AmountOnStock = 42,
                Description = "Some description",
                // Now a list of DiscountDTO
                Discounts = new List<DiscountDTO>
                {
                    new DiscountDTO
                    {
                        DiscountId = discountId,
                        StartDate = new DateTime(2025, 1, 1),
                        EndDate = new DateTime(2025, 1, 15),
                        DiscountPercentage = 10.5
                    }
                },
                Price = 99.99m
            };
            var categoryIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // ACT
            var result = Utilities.CreateValidatedProduct(productDto, categoryIds);

            // ASSERT
            Assert.Equal(productDto.ProductId, result.ProductId);
            Assert.Equal(productDto.ProductPicture, result.ProductPicture);
            Assert.Equal(productDto.ProductName, result.ProductName);
            Assert.Equal(productDto.AmountOnStock, result.AmountOnStock);
            Assert.Equal(categoryIds, result.SelectedCategoryIds);
            Assert.Equal(productDto.Description, result.Description);

            // Check that discounts were copied correctly
            Assert.NotNull(result.Discounts);
            Assert.Single(result.Discounts);
            Assert.Equal(discountId, result.Discounts[0].DiscountId);
            Assert.Equal(10.5, result.Discounts[0].DiscountPercentage);
            Assert.Equal(new DateTime(2025, 1, 1), result.Discounts[0].StartDate);
            Assert.Equal(new DateTime(2025, 1, 15), result.Discounts[0].EndDate);

            Assert.Equal(productDto.Price, result.Price);
        }

        [Fact]
        public void CreateProductToStoreDTO_ShouldMapProperties()
        {
            // ARRANGE
            var discountId = Guid.NewGuid();
            var validatedProduct = new ValidatedProduct
            {
                ProductId = Guid.NewGuid(),
                ProductPicture = "validated_pic.png",
                ProductName = "Validated Product",
                AmountOnStock = 100,
                Description = "Validated desc",
                // Now a list of DiscountDTO
                Discounts = new List<DiscountDTO>
                {
                    new DiscountDTO
                    {
                        DiscountId = discountId,
                        StartDate = new DateTime(2030, 5, 1),
                        EndDate = new DateTime(2030, 5, 31),
                        DiscountPercentage = 25.0
                    }
                },
                Price = 999.99m,
                SelectedCategoryIds = new List<Guid> { Guid.NewGuid() } // Not directly used in method
            };

            var categoryList = new List<CategoryDTO>
            {
                new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Category A" },
                new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Category B" }
            };

            // ACT
            var result = Utilities.CreateProductToStoreDTO(validatedProduct, categoryList);

            // ASSERT
            Assert.Equal(validatedProduct.ProductName, result.ProductName);
            Assert.Equal(validatedProduct.ProductPicture, result.ProductPicture);
            Assert.Equal(validatedProduct.AmountOnStock, result.AmountOnStock);
            Assert.Equal(validatedProduct.Description, result.Description);
            Assert.Equal(categoryList, result.Categories);

            // Check that discounts were copied correctly
            Assert.NotNull(result.Discounts);
            Assert.Single(result.Discounts);
            Assert.Equal(discountId, result.Discounts[0].DiscountId);
            Assert.Equal(25.0, result.Discounts[0].DiscountPercentage);
            Assert.Equal(new DateTime(2030, 5, 1), result.Discounts[0].StartDate);
            Assert.Equal(new DateTime(2030, 5, 31), result.Discounts[0].EndDate);

            Assert.Equal(validatedProduct.Price, result.Price);
            Assert.Equal(validatedProduct.ProductId, result.ProductId);
        }

        [Theory]
        [InlineData(0, "0,00 €")]
        [InlineData(123.45, "123,45 €")]
        [InlineData(1234.56, "1.234,56 €")]
        [InlineData(9999999.99, "9.999.999,99 €")]
        public void FormatAsEuro_ShouldReturnExpectedString(decimal input, string expected)
        {
            // ACT
            var result = Utilities.FormatAsEuro(input);

            // ASSERT
            Assert.Equal(expected, result);
        }
    }
}
