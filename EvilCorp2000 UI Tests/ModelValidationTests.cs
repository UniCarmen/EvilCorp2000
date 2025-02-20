using EvilCorp2000.UIModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvilCorp2000_UI_Tests
{
    public class ModelValidationTests
    {
        [Fact]
        public void ProductName_ShouldFailValidation_WhenEmpty()
        {
            // Arrange
            var model = new ValidatedProduct { ProductName = "" };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.ProductName) };

            // Act
            var isValid = Validator.TryValidateProperty(model.ProductName, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Product Name required");
        }

        [Fact]
        public void Price_ShouldFailValidation_WhenNegative()
        {
            // Arrange
            var model = new ValidatedProduct { Price = -10m };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.Price) };

            // Act
            var isValid = Validator.TryValidateProperty(model.Price, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Price must be at least 0.");
        }

        [Fact]
        public void AmountOnStock_ShouldFailValidation_WhenNegative()
        {
            // Arrange
            var model = new ValidatedProduct { AmountOnStock = -1 };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.AmountOnStock) };

            // Act
            var isValid = Validator.TryValidateProperty(model.AmountOnStock, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Amount on stock cannot be negative.");
        }

        [Fact]
        public void SelectedCategoryIds_ShouldFailValidation_WhenEmpty()
        {
            // Arrange
            var model = new ValidatedProduct { SelectedCategoryIds = new List<Guid>() };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.SelectedCategoryIds) };

            // Act
            var isValid = Validator.TryValidateProperty(model.SelectedCategoryIds, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Please select at least one category.");
        }

        [Fact]
        public void SelectedCategoryIds_ShouldPassValidation_WhenAtLeastOneCategoryExists()
        {
            // Arrange
            var model = new ValidatedProduct { SelectedCategoryIds = new List<Guid> { Guid.NewGuid() } };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.SelectedCategoryIds) };

            // Act
            var isValid = Validator.TryValidateProperty(model.SelectedCategoryIds, context, validationResults);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void StartDate_ShouldFailValidation_WhenNull()
        {
            // Arrange
            var model = new ValidatedDiscount { StartDate = null };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.StartDate) };

            // Act
            var isValid = Validator.TryValidateProperty(model.StartDate, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Start Date Required (min: today)");
        }

        [Fact]
        public void StartDate_ShouldFailValidation_WhenInThePast()
        {
            // Arrange
            var model = new ValidatedDiscount { StartDate = DateTime.Today.AddDays(-1) };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.StartDate) };

            // Act
            var isValid = Validator.TryValidateProperty(model.StartDate, context, validationResults);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void StartDate_ShouldPassValidation_WhenTodayOrFuture()
        {
            // Arrange
            var model = new ValidatedDiscount { StartDate = DateTime.Today };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.StartDate) };

            // Act
            var isValid = Validator.TryValidateProperty(model.StartDate, context, validationResults);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void EndDate_ShouldFailValidation_WhenBeforeStartDate()
        {
            // Arrange
            var model = new ValidatedDiscount
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(-1)
            };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.EndDate) };

            // Act
            var isValid = Validator.TryValidateProperty(model.EndDate, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "End Date Required (must be after Start Date)");
        }

        [Fact]
        public void EndDate_ShouldPassValidation_WhenAfterStartDate()
        {
            // Arrange
            var model = new ValidatedDiscount
            {
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.EndDate) };

            // Act
            var isValid = Validator.TryValidateProperty(model.EndDate, context, validationResults);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void DiscountPercentage_ShouldFailValidation_WhenZeroOrNegative()
        {
            // Arrange
            var model = new ValidatedDiscount { DiscountPercentage = 0 };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.DiscountPercentage) };

            // Act
            var isValid = Validator.TryValidateProperty(model.DiscountPercentage, context, validationResults);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, v => v.ErrorMessage == "Discount Percentage required must be over 0 %");
        }

        [Fact]
        public void DiscountPercentage_ShouldPassValidation_WhenPositive()
        {
            // Arrange
            var model = new ValidatedDiscount { DiscountPercentage = 10 };
            var validationResults = new List<ValidationResult>();
            var context = new ValidationContext(model) { MemberName = nameof(model.DiscountPercentage) };

            // Act
            var isValid = Validator.TryValidateProperty(model.DiscountPercentage, context, validationResults);

            // Assert
            Assert.True(isValid);
        }
    }
}
