using DataAccess.DBContexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace DatabaseTests
{
    public class ProductTests
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
        public async Task GetProductById_ShouldThrowException_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetProductById(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetProductById_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetProductById(Guid.Empty));
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

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteProduct(Guid.Empty));
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
        public async Task GetProductByIdWithCategoriesAnsdDiscounts_ShouldThrowException_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.GetProductById(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetProductByIdWithCategoriesAnsdDiscounts_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.GetProductById(Guid.Empty));
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
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.SaveProductPicture(nonExistentProductId, newPicture));
        }

        [Fact]
        public async Task SaveProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            string newPicture = "updated_picture.jpg";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.SaveProductPicture(Guid.Empty, newPicture));
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
        public async Task DeleteProductPicture_ShouldThrowException_WhenProductDoesNotExist()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            var nonExistentProductId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.DeleteProductPicture(nonExistentProductId));
        }

        [Fact]
        public async Task DeleteProductPicture_ShouldThrowException_WhenProductIdIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new ProductRepository(context, NullLogger<ProductRepository>.Instance);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteProductPicture(Guid.Empty));
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
