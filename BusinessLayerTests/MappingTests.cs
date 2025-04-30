using BusinessLayer.Mappings;
using BusinessLayer.Models;
using DataAccess.Entities;
using static BusinessLayerTests.TestFactory;

namespace BusinessLayerTests
{
    public class MappingTests
    {
        private readonly CategoryMappings _categoryMapper = new();
        private readonly DiscountMappings _discountMapper = new();
        private readonly ProductMappings _productMapper = new();

        [Fact]
        public void CategoryEntityToCategoryModel_ShouldMapCorrectly()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory("Weapons");

            // Act
            var result = _categoryMapper.CategoryToCategoryDto(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(category.CategoryId, result.CategoryId);
            Assert.Equal(category.CategoryName, result.CategoryName);
        }

        [Fact]
        public void CategoryDtoToCategory_ShouldMapCorrectly()
        {
            // Arrange
            var categoryDto = TestDataFactory.CreateCategoryDTO("Armor");

            // Act
            var result = _categoryMapper.CategoryDtoToCategory(categoryDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryDto.CategoryId, result.CategoryId);
            Assert.Equal(categoryDto.CategoryName, result.CategoryName);
        }

        [Fact]
        public void CategoryEntityToCategoryModel_ShouldThrowException_WhenInputIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _categoryMapper.CategoryToCategoryDto(null));
        }

        [Fact]
        public void CategoryDtoToCategory_ShouldThrowException_WhenInputIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _categoryMapper.CategoryDtoToCategory(null));
        }


        [Fact]
        public void DiscountToDiscountDTO_ShouldMapCorrectly()
        {
            // Arrange
            var discount = TestDataFactory.CreateDiscount(10);

            // Act
            var result = _discountMapper.DiscountToDiscountDTO(discount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(discount.DiscountId, result.DiscountId);
            Assert.Equal(discount.DiscountPercentage, result.DiscountPercentage);
            Assert.Equal(discount.StartDate, result.StartDate);
            Assert.Equal(discount.EndDate, result.EndDate);
        }

        [Fact]
        public void DiscountToDiscountDTO_ShouldThrowException_WhenInputIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _discountMapper.DiscountToDiscountDTO(null));
        }

        [Fact]
        public void DiscountDTOToDiscount_ShouldMapCorrectly()
        {
            // Arrange
            var discountDto = TestDataFactory.CreateDiscountDTO(15);

            Guid productId = Guid.NewGuid();

            // Act
            var result = _discountMapper.DiscountDTOToDiscount(discountDto, productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(discountDto.DiscountId, result.DiscountId);
            Assert.Equal(discountDto.DiscountPercentage, result.DiscountPercentage);
            Assert.Equal(discountDto.StartDate, result.StartDate);
            Assert.Equal(discountDto.EndDate, result.EndDate);
        }

        [Fact]
        public void DiscountDTOToDiscount_ShouldThrowException_WhenDiscountDTOIsNull()
        {
            // Arrange
            Guid productId = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _discountMapper.DiscountDTOToDiscount(null, productId));
        }

        [Fact]
        public void DiscountDTOToDiscount_ShouldThrowException_WhenProductIdIsEmpty()
        {
            // Arrange
            var discountDto = TestDataFactory.CreateDiscountDTO(20);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _discountMapper.DiscountDTOToDiscount(discountDto, Guid.Empty));
        }

        [Fact]
        public void DiscountDTOToDiscount_ShouldGenerateNewDiscountId_WhenDiscountIdIsEmpty()
        {
            // Arrange
            var discountDto = TestDataFactory.CreateDiscountDTO(10);
            discountDto.DiscountId = Guid.Empty;

            Guid productId = Guid.NewGuid();

            // Act
            var result = _discountMapper.DiscountDTOToDiscount(discountDto, productId);

            // Assert
            Assert.NotEqual(Guid.Empty, result.DiscountId);
        }


        [Fact]
        public void ProductToProductForSale_ShouldThrowException_WhenProductEntityIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _productMapper.ProductToProductForSaleDto(null, new Discount()));
        }

        [Fact]
        public void ProductToProductForSale_ShouldApplyDiscount_WhenDiscountIsValid()
        {
            // Arrange
            var product = TestDataFactory.CreateProduct("Test Product", 100, new List<Category> { new Category { CategoryName = "Weapons" } });

            var discount = TestDataFactory.CreateDiscount(10);

            // Act
            var result = _productMapper.ProductToProductForSaleDto(product, discount);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(90, result.DiscountedPrice); // 10% Rabatt abgezogen
            Assert.Equal(10, result.Discount);
        }

        [Fact]
        public void ProductToProductForSale_ShouldReturnFullPrice_WhenNoDiscount()
        {
            // Arrange
            var product = TestDataFactory.CreateProduct("Test Product", 100, new List<Category> { new Category { CategoryName = "Weapons" } });

            // Act
            var result = _productMapper.ProductToProductForSaleDto(product, null);

            // Assert
            Assert.Null(result.DiscountedPrice); // Kein Rabatt
            Assert.Equal(0, result.Discount); // Rabattprozent ebenfalls 0
        }


        [Fact]
        public void ProductManagementProductToProductEntity_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() 
                => _productMapper.ProductManagementProductDtoToProductEntity(null, new List<Category>(), new List<Discount>()));
        }

        [Fact]
        public void ProductManagementProductToProductEntity_ShouldThrowException_WhenCategoriesAreEmpty()
        {
            // Arrange
            var pmProduct = new ProductManagementProductDTO { ProductId = Guid.NewGuid(), ProductName = "Test" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() 
                => _productMapper.ProductManagementProductDtoToProductEntity(pmProduct, new List<Category>(), new List<Discount>()));
        }

        [Fact]
        public void ProductManagementProductToProductEntity_ShouldGenerateNewId_WhenProductIdIsEmpty()
        {
            // Arrange
            var pmProduct = new ProductManagementProductDTO { ProductId = Guid.Empty, ProductName = "Test" };
            var categories = new List<Category> { new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } };
            var discounts = new List<Discount>();

            // Act
            var result = _productMapper.ProductManagementProductDtoToProductEntity(pmProduct, categories, discounts);

            // Assert
            Assert.NotEqual(Guid.Empty, result.ProductId);
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _productMapper.ProductToProductManagementProductDto(null, new List<DiscountDTO>(), new List<CategoryDTO>()));
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldThrowException_WhenCategoriesAreEmpty()
        {
            // Arrange
            var product = new Product { ProductId = Guid.NewGuid(), ProductName = "Test" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _productMapper.ProductToProductManagementProductDto(product, new List<DiscountDTO>(), new List<CategoryDTO>()));
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldMapCorrectly()
        {
            // Arrange
            var product = TestDataFactory.CreateProduct("Test Product", 100, new List<Category> { new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } });
           
            var categories = new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } };
            var discounts = new List<DiscountDTO>();

            // Act
            var result = _productMapper.ProductToProductManagementProductDto(product, discounts, categories);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductName, result.ProductName);
            Assert.Equal(product.ProductPrice, result.Price);
            Assert.Equal(product.ProductDescription, result.Description);
            Assert.NotEmpty(result.Categories);
        }
    }
}

