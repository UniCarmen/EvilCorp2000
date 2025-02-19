using BusinessLayer.Models;
using BusinessLayer.Services;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayerTests
{
    public class ValidationTests
    {
        [Fact]
        public void ValidateProduct_ShouldThrowException_WhenProductNameIsNotUnique()
        {
            // Arrange
            var product = new ProductManagementProductDTO { ProductName = "Test Product", Price = 10.0m, AmountOnStock = 5 };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateProduct(product, false));
            Assert.Contains("Product name must be unique.", exception.Message);
        }

        [Fact]
        public void ValidateProduct_ShouldThrowException_WhenPriceIsZeroOrNegative()
        {
            // Arrange
            var product = new ProductManagementProductDTO { ProductName = "Test Product", Price = 0.0m, AmountOnStock = 5 };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateProduct(product, true));
            Assert.Contains("Price must be greater than 0.", exception.Message);
        }

        [Fact]
        public void ValidateProduct_ShouldThrowException_WhenAmountOnStockIsNegative()
        {
            // Arrange
            var product = new ProductManagementProductDTO { ProductName = "Test Product", Price = 10.0m, AmountOnStock = -1 };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateProduct(product, true));
            Assert.Contains("Amount on stock cannot be negative.", exception.Message);
        }

        [Fact]
        public void ValidateProduct_ShouldThrowException_WhenMultipleErrorsOccur()
        {
            // Arrange
            var product = new ProductManagementProductDTO { ProductName = "Test Product", Price = -5.0m, AmountOnStock = -1 };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateProduct(product, false));
            Assert.Contains("Product name must be unique.", exception.Message);
            Assert.Contains("Price must be greater than 0.", exception.Message);
            Assert.Contains("Amount on stock cannot be negative.", exception.Message);
        }

        [Fact]
        public void ValidateProduct_ShouldNotThrowException_WhenProductIsValid()
        {
            // Arrange
            var product = new ProductManagementProductDTO { ProductName = "Test Product", Price = 10.0m, AmountOnStock = 5 };

            // Act & Assert
            var exception = Record.Exception(() => ValidationService.ValidateProduct(product, true));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateDiscountAsync_ShouldThrowException_WhenStartOrEndDateIsInThePast()
        {
            // Arrange
            var discount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 10,
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today.AddDays(5)
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateDiscountAsync(discount, new List<DiscountDTO>()));
            Assert.Contains("Start and End dates must not be in the past.", exception.Message);
        }

        [Fact]
        public void ValidateDiscountAsync_ShouldThrowException_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var discount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 10,
                StartDate = DateTime.Today.AddDays(5),
                EndDate = DateTime.Today.AddDays(3)
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateDiscountAsync(discount, new List<DiscountDTO>()));
            Assert.Contains("End Date must be after Start Date.", exception.Message);
        }

        [Fact]
        public void ValidateDiscountAsync_ShouldThrowException_WhenDiscountPercentageIsZeroOrNegative()
        {
            // Arrange
            var discount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 0,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5)
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateDiscountAsync(discount, new List<DiscountDTO>()));
            Assert.Contains("Discount Percentage must be greater than 0.", exception.Message);
        }

        [Fact]
        public void ValidateDiscountAsync_ShouldThrowException_WhenDiscountsOverlap()
        {
            // Arrange
            var existingDiscounts = new List<DiscountDTO>
            {
                new DiscountDTO
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = 10,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(5)
                }
            };

            var newDiscount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 15,
                StartDate = DateTime.Today.AddDays(3), // Overlapping Start Date
                EndDate = DateTime.Today.AddDays(6)
            };

            // Act & Assert
            var exception = Assert.Throws<ValidationException>(() => ValidationService.ValidateDiscountAsync(newDiscount, existingDiscounts));
            Assert.Contains("Discount overlaps with an existing discount.", exception.Message);
        }

        [Fact]
        public void ValidateDiscountAsync_ShouldNotThrowException_WhenDiscountIsValid()
        {
            // Arrange
            var existingDiscounts = new List<DiscountDTO>
            {
                new DiscountDTO
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = 10,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(5)
                }
            };

            var newDiscount = new DiscountDTO
            {
                DiscountId = Guid.NewGuid(),
                DiscountPercentage = 15,
                StartDate = DateTime.Today.AddDays(6), // No overlap
                EndDate = DateTime.Today.AddDays(10)
            };

            // Act & Assert
            var exception = Record.Exception(() => ValidationService.ValidateDiscountAsync(newDiscount, existingDiscounts));
            Assert.Null(exception);
        }
    }
}
