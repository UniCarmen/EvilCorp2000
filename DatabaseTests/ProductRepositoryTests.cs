using DataAccess.DBContexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using static Shared.Utilities.Utilities;

namespace DatabaseTests
{
    public class ProductRepositoryTests
    {
        private EvilCorp2000Context CreateInMemoryDbContext()
        { var options = new DbContextOptionsBuilder<EvilCorp2000Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new EvilCorp2000Context(options);
        }


        private static Category CreateCategory1() { return new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" }; }
        private static Category CreateCategory2() { return new Category { CategoryId = Guid.NewGuid(), CategoryName = "Armor" }; } 
        private static Discount CreateDiscount1() { return new Discount { DiscountId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(10), DiscountPercentage = 10 }; } 
        private static Discount CreateDiscount2() { return new Discount { DiscountId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(20), DiscountPercentage = 20 }; } 

        private static Product CreateProduct()
        {
            return
                new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Laser Cannon",
                    ProductPrice = 500.00m,
                    AmountOnStock = 10,
                    ProductDescription = "A high-powered laser cannon.",
                    ProductPicture = "laser_cannon.jpg",
                    Categories = new List<Category> { CreateCategory1(), CreateCategory2() },
                    Discounts = new List<Discount> { CreateDiscount1(), CreateDiscount2() }
                };
        } 

        [Fact]
        public async Task DeletingProduct_ShouldDeleteRelatedDiscounts()
        {
            // Arrange
            //using stellt sicher, dass DB Ress nach Nutzung wieder freigegeben werden, selbst bei einer Exception
            using var context = CreateInMemoryDbContext();

            context.Products.Add(CreateProduct());
            await context.SaveChangesAsync();
            

            // Act
            var logger = NullLogger<ProductRepository>.Instance; // der macht nichts, da auch nichts geloggt werden soll, ich den aber brauche
            var repo = new ProductRepository(context, logger);
            var testProduct = await context.Products.Include(p => p.Discounts)
                .FirstOrDefaultAsync(p => p.ProductName == "Laser Cannon");

            await repo.DeleteProduct(testProduct.ProductId);
            

            // Assert
                var remainingDiscounts = await context.Discounts.ToListAsync();
                var remainingProducts = await context.Products.ToListAsync();

                Assert.Empty(remainingDiscounts);
                Assert.Empty(remainingProducts);
            
        }


        [Fact]
        public async Task AddProduct_ShouldSaveProductWithCategoriesAndDiscounts()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act
            var product = CreateProduct();
            await repository.AddProduct(product);

            // Assert
            var savedProduct = await context.Products
                .Include(p => p.Categories)
                .Include(p => p.Discounts)
                .FirstOrDefaultAsync(p => p.ProductId == product.ProductId);

            Assert.NotNull(savedProduct);
            Assert.Equal("Laser Cannon", savedProduct.ProductName);
            Assert.Equal(500.00m, savedProduct.ProductPrice);
            Assert.Equal(10, savedProduct.AmountOnStock);
            Assert.Equal(2, savedProduct.Categories.Count);
            Assert.Contains(savedProduct.Categories, c => c.CategoryName == "Weapons");
            Assert.Contains(savedProduct.Categories, c => c.CategoryName == "Armor");
            Assert.Equal(2, savedProduct.Discounts.Count);
            Assert.Contains(savedProduct.Discounts, d => d.DiscountPercentage == 10);
            Assert.Contains(savedProduct.Discounts, d => d.DiscountPercentage == 20);
        }


        [Fact]
        public async Task AddProduct_ShouldThrowException_WhenProductIsNull()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddProduct(null));
        }


        [Fact]
        public async Task GetProduct_ProductNotFound_ThrowsArgumentNullException()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.GetProduct(Guid.NewGuid(), "Product not found"));

            Assert.Contains("Product not found", ex.Message); //da eigene Formatierung von .NET bei Fehlernachrichten
            Assert.Equal("product", ex.ParamName);
        }

        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act
            var result = await repository.GetProduct(product.ProductId, "Product not found");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(product.ProductId, result.ProductId);
        }

        [Fact]
        public async Task GetProduct_WithEmptyId_ThrowsArgumentNullException()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.GetProduct(Guid.NewGuid(), "Product not found"));
        }

        //public async Task DeleteProductPicture(Guid productId)
        //{
        //    try
        //    {
        //        productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

        //        var product = await GetProductById(productId);

        //        if (product == null)
        //        {
        //            var errorMessage = "Product from database is null";
        //            _logger.LogWarning(errorMessage);
        //            throw new ArgumentNullException(errorMessage);
        //        }

        //        product.ProductPicture = null;

        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        _logger.LogError(ex, $"Database error while deleting picture for product with id {productId}");
        //        throw;
        //    }
        //}

        [Fact]
        public async Task DeleteProductPicture_ProductNotFound_ThrowsArgumentNullException()
        {
            // Arrange
            var context = CreateInMemoryDbContext();

            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.DeleteProductPicture(Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteProductPicture_WithValidId_ShouldDeleteProductPicture()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act
            await repository.DeleteProductPicture(product.ProductId);

            var result = await context.Products.FindAsync(product.ProductId);

            // Assert
            Assert.NotNull(result);
            Assert.True(string.IsNullOrEmpty(result.ProductPicture), "Product picture should be null or empty after deletion.");
            // Assert

        }

        [Fact]
        public async Task DeleteProductPicture_ShouldThrowException_WhenIdIsInvalid()
        {
            // Arrange
            var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repository.DeleteProductPicture(Guid.NewGuid()));
        }



        [Fact]
        public async Task GetProductById_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() => repository.GetProductById(Guid.Empty));
        }

        [Fact]
        public async Task UpdateProduct_ShouldSaveChanges_WhenProductExists()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();

            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            product.ProductName = "Updated Name";
            await repository.UpdateProduct(product);

            // Assert
            var updatedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Equal("Updated Name", updatedProduct.ProductName);
        }

        [Fact]
        public async Task UpdateProduct_ShouldThrowException_WhenProductIsNull()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateProduct(null));
        }

        
        [Fact]
        public async Task DeleteProduct_ShouldRemoveProduct_WhenProductExists()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            var product = CreateProduct();

            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteProduct(product.ProductId);

            // Assert
            var deletedProduct = await context.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            Assert.Null(deletedProduct);
        }


        [Fact]
        public async Task DeleteProduct_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() => repository.DeleteProduct(Guid.Empty));
        }


        [Fact]
        public async Task GetProductByIdWithCategoriesAnsdDiscounts_ShouldReturnProductWithRelations_WhenProductExists()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();

            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductByIdWithCategoriesAnsdDiscounts(product.ProductId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Laser Cannon", result.ProductName);
            Assert.Equal(500.00m, result.ProductPrice);
            Assert.Equal(2, result.Discounts.Count);
            Assert.Equal(2, result.Categories.Count);
        }


        [Fact]
        public async Task GetProductByIdWithCategoriesAnsdDiscounts_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() => repository.GetProductById(Guid.Empty));
        }


        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            context.Products.Add(CreateProduct());
            context.Products.Add(CreateProduct());
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllProductsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count); // Prüft, dass zwei Produkte zurückgegeben wurden.
            Assert.Contains(result, p => p.ProductName == "Laser Cannon");
        }

        [Theory]
        [InlineData(ProductSortOrder.PriceAsc)]
        [InlineData(ProductSortOrder.PriceDesc)]
        [InlineData(ProductSortOrder.DiscountAsc)]
        [InlineData(ProductSortOrder.DiscountDesc)]
        [InlineData(ProductSortOrder.RatingDesc)]
        [InlineData(ProductSortOrder.RatingAsc)]
        [InlineData(ProductSortOrder.NameDesc)]
        [InlineData(ProductSortOrder.NameAsc)]
        [InlineData(ProductSortOrder.StockAsc)]
        [InlineData(ProductSortOrder.StockDesc)]
        public async Task GetAllProductsAsync_ShouldReturnAllProductsSorted(ProductSortOrder sortOrder)
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product1 = CreateProduct(); 
            var product2 = CreateProduct(); 
            var product3 = CreateProduct(); 
            
            product1.ProductPrice = 100m;
            product1.ProductName = "Laser Cannon";
            var discount1 = CreateDiscount1();
            discount1.StartDate = DateTime.Today.AddDays(-1);
            discount1.EndDate = DateTime.Today.AddDays(1);
            discount1.DiscountPercentage = 10.0;
            product1.Discounts = [discount1];
            product1.Rating = 1;
            product1.AmountOnStock = 3;

            product2.ProductPrice = 200m;
            product2.ProductName = "Plasma Rifle";
            var discount2 = CreateDiscount1();
            discount2.StartDate = DateTime.Today.AddDays(-1);
            discount2.EndDate = DateTime.Today.AddDays(1);
            discount2.DiscountPercentage = 20.0;
            product2.Discounts = [discount2];
            product2.Rating = null;
            product2.AmountOnStock = 10;

            product3.ProductPrice = 50m;
            product3.ProductName = "Nano Sword";
            product3.Discounts = [];
            product3.Rating = 5;
            product3.AmountOnStock = 0;

            context.Products.AddRange(product1, product2, product3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllProductsAsync(sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count); //3 produkte vorhanden?

            // Testet die Sortierung basierend auf dem SortOrder
            switch (sortOrder)
            {
                case ProductSortOrder.PriceAsc:
                    Assert.Equal(new[] { "Nano Sword", "Laser Cannon", "Plasma Rifle" }, result.Select(p => p.ProductName).ToArray());
                    break;

                case ProductSortOrder.PriceDesc:
                    Assert.Equal(new[] { "Plasma Rifle", "Laser Cannon", "Nano Sword" }, result.Select(p => p.ProductName).ToArray());
                    break;

                case ProductSortOrder.DiscountAsc:
                    Assert.Equal(new[] { "Laser Cannon", "Plasma Rifle", "Nano Sword" }, result.Select(p => p.ProductName).ToArray()); // Kein Rabatt -> 10% -> 20%
                    break;

                case ProductSortOrder.DiscountDesc:
                    Assert.Equal(new[] { "Plasma Rifle", "Laser Cannon", "Nano Sword" }, result.Select(p => p.ProductName).ToArray()); // 20% -> 10% -> Kein Rabatt
                    break;

                case ProductSortOrder.RatingDesc:
                    Assert.Equal(new[] { "Nano Sword", "Laser Cannon", "Plasma Rifle" }, result.Select(p => p.ProductName).ToArray()); // 5 -> 1 -> Kein Rating
                    break;

                case ProductSortOrder.RatingAsc:
                    Assert.Equal(new[] { "Laser Cannon", "Nano Sword", "Plasma Rifle" }, result.Select(p => p.ProductName).ToArray()); // 5 -> 1 -> Kein Rating
                    break;

                case ProductSortOrder.NameAsc:
                    Assert.Equal(new[] { "Laser Cannon", "Nano Sword", "Plasma Rifle" }, result.Select(p => p.ProductName).ToArray()); 
                    break;

                case ProductSortOrder.NameDesc:
                    Assert.Equal(new[] { "Plasma Rifle", "Nano Sword", "Laser Cannon" }, result.Select(p => p.ProductName).ToArray()); 
                    break;

                case ProductSortOrder.StockAsc:
                    Assert.Equal(new[] { "Nano Sword", "Laser Cannon", "Plasma Rifle" }, result.Select(p => p.ProductName).ToArray()); // 0 -> 3 -> 10
                    break;

                case ProductSortOrder.StockDesc:
                    Assert.Equal(new[] { "Plasma Rifle", "Laser Cannon", "Nano Sword" }, result.Select(p => p.ProductName).ToArray()); // 10 -> 3 -> 0
                    break;
            }
        }


        [Fact]
        public async Task SaveProductPicture_ShouldUpdateProductPicture_WhenProductExists()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            string newPicture = "updated_picture.jpg";

            // Act
            await repository.SaveProductPicture(product.ProductId, newPicture);

            // Assert
            var updatedProduct = await context.Products.FindAsync(product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Equal(newPicture, updatedProduct.ProductPicture);
        }

        [Fact]
        public async Task SaveProductPicture_ShouldThrowException_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            var nonExistentProductId = Guid.NewGuid();
            string newPicture = "updated_picture.jpg";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.SaveProductPicture(nonExistentProductId, newPicture));
        }

        [Fact]
        public async Task SaveProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            string newPicture = "updated_picture.jpg";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.SaveProductPicture(Guid.Empty, newPicture));
        }

        [Fact]
        public async Task DeleteProductPicture_ShouldRemovePicture_WhenProductExists()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteProductPicture(product.ProductId);

            // Assert
            var updatedProduct = await context.Products.FindAsync(product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Null(updatedProduct.ProductPicture);
        }

        [Fact]
        public async Task DeleteProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => repository.DeleteProductPicture(Guid.Empty));
        }


        [Fact]
        public async Task IsProductNameUniqueAsync_ShouldReturnTrue_WhenProductNameIsUniqueForNewProduct()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange - Keine Produkte in der DB
            string uniqueProductName = "Unique Laser";

            // Act
            var result = await repository.IsProductNameUniqueAsync(uniqueProductName, Guid.Empty);

            // Assert
            Assert.True(result); // Da kein Produkt existiert, muss der Name einzigartig sein
        }


        [Fact]
        public async Task IsProductNameUniqueAsync_ShouldReturnFalse_WhenProductNameAlreadyExistsForNewProduct()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.IsProductNameUniqueAsync("Laser Cannon", Guid.Empty);

            // Assert
            Assert.False(result); // Name existiert bereits
        }


        [Fact]
        public async Task IsProductNameUniqueAsync_ShouldReturnTrue_WhenProductNameBelongsToSameProductOrIsUnique()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product = CreateProduct();
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.IsProductNameUniqueAsync("Laser Cannon", product.ProductId);

            // Assert
            Assert.True(result); // Der Name gehört zum gleichen Produkt oder ist noch nicht vorhanden
        }


        [Fact]
        public async Task IsProductNameUniqueAsync_ShouldReturnFalse_WhenProductNameExistsForAnotherProduct()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Arrange
            var product1 = CreateProduct();
            var product2 = CreateProduct();
            product2.ProductName = "Another Name";
            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.IsProductNameUniqueAsync("Another Name", product1.ProductId);

            // Assert
            Assert.False(result); // Der Name gehört einem anderen Produkt, daher nicht einzigartig
        }


        [Fact]
        public async Task IsProductNameUniqueAsync_ShouldThrowException_WhenNameIsNullOrEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            //INFO ThrowAsync erwartet eine Funktion, die einen Task zurückgibt, deswegen brauche ich eine An. Funktion
            //INFO mit await repository.IsProductNameUniqueAsync("", Guid.Empty) würde die Methode sofort aufgerufen werden und nicht durch ThrowAsync evauliert werden
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.IsProductNameUniqueAsync("", Guid.Empty));
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.IsProductNameUniqueAsync(null, Guid.Empty));
        }

    }
}
