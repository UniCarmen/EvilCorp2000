using DataAccess.DBContexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;


namespace DatabaseTests
{
    public class DiscountRepositoryTests
    {
        private EvilCorp2000Context CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EvilCorp2000Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Jede DB ist isoliert
                .Options;
            return new EvilCorp2000Context(options);
        }

        private static Discount NewDiscount1() { return new Discount { DiscountId = Guid.NewGuid(), ProductId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), DiscountPercentage = 10 }; }
        private static Discount NewDiscount2() { return new Discount { DiscountId = Guid.NewGuid(), ProductId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(10), DiscountPercentage = 20 }; } 

        [Fact]
        public async Task GetDiscountsByProductId_ShouldReturnDiscountsForProduct()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);


            var productId = Guid.NewGuid();
            var unrelatedDiscount = new Discount { DiscountId = Guid.NewGuid(), ProductId = Guid.NewGuid(), StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(15), DiscountPercentage = 30 };

            var dicsount1 = NewDiscount1();
            dicsount1.ProductId = productId;
            var discount2 = NewDiscount2();
            discount2.ProductId = productId;

            context.Discounts.AddRange(dicsount1, discount2, unrelatedDiscount);
            await context.SaveChangesAsync();

            var result = await repository.GetDiscountsByProductId(productId);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, d => Assert.Equal(productId, d.ProductId));
        }

        [Fact]
        public async Task GetDiscountsByProductId_ShouldThrowException_WhenGuidIsEmpty()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentException>(() => repository.GetDiscountsByProductId(Guid.Empty));
        }

        [Fact]
        public async Task GetCurrentDiscountByProductId_ShouldReturnCurrentDiscount()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            var productId = Guid.NewGuid();
            var expiredDiscount = new Discount { DiscountId = Guid.NewGuid(), ProductId = productId, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-5), DiscountPercentage = 15 };
            var validDiscount = new Discount { DiscountId = Guid.NewGuid(), ProductId = productId, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(3), DiscountPercentage = 25 };

            context.Discounts.AddRange(expiredDiscount, validDiscount);
            await context.SaveChangesAsync();

            var result = await repository.GetCurrentDiscountByProductId(productId);

            Assert.NotNull(result);
            Assert.Equal(25, result.DiscountPercentage);
        }

        [Fact]
        public async Task GetCurrentDiscountByProductId_ShouldReturnNull_IfNoActiveDiscount()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            var productId = Guid.NewGuid();
            var expiredDiscount = new Discount { DiscountId = Guid.NewGuid(), ProductId = productId, StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(-5), DiscountPercentage = 15 };

            context.Discounts.Add(expiredDiscount);
            await context.SaveChangesAsync();

            var result = await repository.GetCurrentDiscountByProductId(productId);

            Assert.Null(result);
        }


        [Fact]
        public async Task AddDiscount_ShouldSaveDiscountToDatabase()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            var newDiscount = NewDiscount1();
            await repository.AddDiscount(newDiscount);
            var result = await context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == newDiscount.DiscountId);

            Assert.NotNull(result);
            Assert.Equal(10, result.DiscountPercentage);
        }

        [Fact]
        public async Task AddDiscount_ShouldThrowException_WhenDiscountIsNull()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.AddDiscount(null));
        }

        [Fact]
        public async Task DeleteDiscount_ShouldRemoveDiscountFromDatabase()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            var discount = NewDiscount1();
            context.Discounts.Add(discount);
            await context.SaveChangesAsync();

            await repository.DeleteDiscount(discount);
            var result = await context.Discounts.FirstOrDefaultAsync(d => d.DiscountId == discount.DiscountId);

            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteDiscount_ShouldThrowException_WhenDiscountIsNull()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteDiscount(null));
        }


        [Fact]
        public async Task UpdateDiscounts_ShouldAddNewDiscountsToProduct()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
                Discounts = new List<Discount>()
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var dicsount1 = NewDiscount1();
            dicsount1.ProductId = product.ProductId;
            var discount2 = NewDiscount2();
            discount2.ProductId = product.ProductId;

            List<Discount> newDiscounts = [dicsount1, discount2];

            // Act
            await repository.UpdateDiscounts(product, newDiscounts);

            // Assert
            var updatedProduct = await context.Products.Include(p => p.Discounts).FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Equal(2, updatedProduct.Discounts.Count);
            Assert.Contains(updatedProduct.Discounts, d => d.DiscountPercentage == 10);
            Assert.Contains(updatedProduct.Discounts, d => d.DiscountPercentage == 20);
        }

        [Fact]
        public async Task UpdateDiscounts_ShouldRemoveDiscountsNotInList()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
            };
            var discount1 = NewDiscount1();
            discount1.ProductId = product.ProductId;
            var discount2 = NewDiscount2();
            discount2.ProductId = product.ProductId;


            product.Discounts.Add(discount1);
            product.Discounts.Add(discount2);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            List<Discount> updatedDiscounts = [discount2];

            // Act
            await repository.UpdateDiscounts(product, updatedDiscounts);

            // Assert
            var updatedProduct = await context.Products.Include(p => p.Discounts).FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Single(updatedProduct.Discounts);
        }

        [Fact]
        public async Task UpdateDiscounts_ShouldModifyExistingDiscounts()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            // Arrange
            var product = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = "Test Product",
            };
            var dicsount1 = NewDiscount1();
            dicsount1.ProductId = product.ProductId;
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var updatedDiscount = dicsount1;
            updatedDiscount.DiscountPercentage = 50;

            List<Discount> updatedDiscounts = [updatedDiscount];

            // Act
            await repository.UpdateDiscounts(product, updatedDiscounts);

            // Assert
            var updatedProduct = await context.Products.Include(p => p.Discounts).FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
            Assert.NotNull(updatedProduct);
            Assert.Single(updatedProduct.Discounts);
            Assert.Equal(50, updatedProduct.Discounts.First().DiscountPercentage);
        }

        [Fact]
        public async Task UpdateDiscounts_ShouldThrowException_WhenProductIsNull()
        {
            using var context = CreateInMemoryDbContext();
            var repository = new DiscountRepository(context, NullLogger<DiscountRepository>.Instance);

            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.UpdateDiscounts(null, new List<Discount>()));
        }

    }
}

