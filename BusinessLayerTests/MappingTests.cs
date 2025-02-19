using BusinessLayer.Mappings;
using BusinessLayer.Models;
using DataAccess.Entities;

namespace BusinessLayerTests
{
    public class MappingTests
    {
        private readonly CategoryMappings _categoryMapper = new();
        private readonly DiscountMappings _discountMapper = new();
        private readonly ProductMappings _productMapper = new();

        public static class TestDataFactory
        {
            public static Category CreateCategory(string name)
            {
                return new Category
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = name
                };
            }

            public static CategoryDTO CreateCategoryDTO(string name)
            {
                return new CategoryDTO
                {
                    CategoryId = Guid.NewGuid(),
                    CategoryName = name
                };
            }

            public static Discount CreateDiscount(double percentage)
            {
                return new Discount
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = percentage,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5)
                };
            }

            public static DiscountDTO CreateDiscountDTO(double percentage)
            {
                return new DiscountDTO
                {
                    DiscountId = Guid.NewGuid(),
                    DiscountPercentage = percentage,
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddDays(5)
                };
            }

            public static Product CreateProduct(string name, decimal price, List<Category> categories = null, List<Discount> discounts = null)
            {
                return new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = name,
                    ProductPrice = price,
                    ProductDescription = "Test Description",
                    ProductPicture = "test.jpg",
                    AmountOnStock = 10,
                    Categories = categories ?? new List<Category> { CreateCategory("Weapons") },
                    Discounts = discounts ?? new List<Discount> { CreateDiscount(10) }
                };
            }

            public static ProductManagementProductDTO CreateProductManagementDTO(string name, decimal price, List<CategoryDTO> categories = null, List<DiscountDTO> discounts = null)
            {
                return new ProductManagementProductDTO
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = name,
                    Price = price,
                    Description = "Test Description",
                    ProductPicture = "test.jpg",
                    AmountOnStock = 10,
                    Categories = categories ?? new List<CategoryDTO> { CreateCategoryDTO("Weapons") },
                    Discounts = discounts ?? new List<DiscountDTO> { CreateDiscountDTO(10) }
                };
            }
        }


        [Fact]
        public void CategoryEntityToCategoryModel_ShouldMapCorrectly()
        {
            // Arrange
            var category = TestDataFactory.CreateCategory("Weapons");

            // Act
            var result = _categoryMapper.CategoryEntityToCategoryModel(category);

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
            Assert.Throws<ArgumentNullException>(() => _categoryMapper.CategoryEntityToCategoryModel(null));
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
            Assert.Throws<ArgumentNullException>(() => _productMapper.ProductToProductForSale(null, new Discount()));
        }

        [Fact]
        public void ProductToProductForSale_ShouldApplyDiscount_WhenDiscountIsValid()
        {
            // Arrange
            var product = TestDataFactory.CreateProduct("Test Product", 100, new List<Category> { new Category { CategoryName = "Weapons" } });

            var discount = TestDataFactory.CreateDiscount(10);

            // Act
            var result = _productMapper.ProductToProductForSale(product, discount);

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
            var result = _productMapper.ProductToProductForSale(product, null);

            // Assert
            Assert.Equal(0, result.DiscountedPrice); // Kein Rabatt
            Assert.Equal(0, result.Discount); // Rabattprozent ebenfalls 0
        }


        [Fact]
        public void ProductManagementProductToProductEntity_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() 
                => _productMapper.ProductManagementProductToProductEntity(null, new List<Category>(), new List<Discount>()));
        }

        [Fact]
        public void ProductManagementProductToProductEntity_ShouldThrowException_WhenCategoriesAreEmpty()
        {
            // Arrange
            var pmProduct = new ProductManagementProductDTO { ProductId = Guid.NewGuid(), ProductName = "Test" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() 
                => _productMapper.ProductManagementProductToProductEntity(pmProduct, new List<Category>(), new List<Discount>()));
        }

        [Fact]
        public void ProductManagementProductToProductEntity_ShouldGenerateNewId_WhenProductIdIsEmpty()
        {
            // Arrange
            var pmProduct = new ProductManagementProductDTO { ProductId = Guid.Empty, ProductName = "Test" };
            var categories = new List<Category> { new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } };
            var discounts = new List<Discount>();

            // Act
            var result = _productMapper.ProductManagementProductToProductEntity(pmProduct, categories, discounts);

            // Assert
            Assert.NotEqual(Guid.Empty, result.ProductId);
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldThrowException_WhenProductIsNull()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => _productMapper.ProductToProductManagementProduct(null, new List<DiscountDTO>(), new List<CategoryDTO>()));
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldThrowException_WhenCategoriesAreEmpty()
        {
            // Arrange
            var product = new Product { ProductId = Guid.NewGuid(), ProductName = "Test" };

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _productMapper.ProductToProductManagementProduct(product, new List<DiscountDTO>(), new List<CategoryDTO>()));
        }

        [Fact]
        public void ProductToProductManagementProduct_ShouldMapCorrectly()
        {
            // Arrange
            var product = TestDataFactory.CreateProduct("Test Product", 100, new List<Category> { new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } });
           
            var categories = new List<CategoryDTO> { new CategoryDTO { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" } };
            var discounts = new List<DiscountDTO>();

            // Act
            var result = _productMapper.ProductToProductManagementProduct(product, discounts, categories);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductName, result.ProductName);
            Assert.Equal(product.ProductPrice, result.Price);
            Assert.Equal(product.ProductDescription, result.Description);
            Assert.NotEmpty(result.Categories);
        }
    }
}

