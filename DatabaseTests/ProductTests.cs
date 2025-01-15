using EvilCorp2000.DBContexts;
using EvilCorp2000.Entities;
using EvilCorp2000.Repositories;
using Microsoft.EntityFrameworkCore;
using System;

namespace DatabaseTests
{
    public class ProductTests
    {
        private readonly DbContextOptions<EvilCorp2000Context> _dbContextOptions;

        public ProductTests() //Konstruktor erstellt eine InMemoryDatabase
        {
            _dbContextOptions = new DbContextOptionsBuilder<EvilCorp2000Context>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
        }

        [Fact]
        public async Task DeletingProduct_ShouldDeleteRelatedDiscounts()
        {
            // Arrange
            //using stellt sicher, dass DB Ress nach Nutzung wieder freigegeben werden, selbst bei einer Exception
            using (var context = new EvilCorp2000Context(_dbContextOptions))
            {
                var product = new Product
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    AmountOnStock = 1,
                    ProductPrice = 100.00m,
                    Discounts = new List<Discount>
                {
                    new Discount
                    {
                        DiscountId = Guid.NewGuid(),
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddDays(10),
                        DiscountPercentage = 10
                    },
                    new Discount
                    {
                        DiscountId = Guid.NewGuid(),
                        StartDate = DateTime.UtcNow.AddDays(15),
                        EndDate = DateTime.UtcNow.AddDays(20),
                        DiscountPercentage = 15
                    }
                }
                };

                context.Products.Add(product);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new EvilCorp2000Context(_dbContextOptions))
            {
                var repo = new ProductRepository(context);
                var product = await context.Products.Include(p => p.Discounts)
                    .FirstOrDefaultAsync(p => p.ProductName == "Test Product");

                await repo.DeleteProduct(product.ProductId);
            }

            // Assert
            using (var context = new EvilCorp2000Context(_dbContextOptions))
            {
                var remainingDiscounts = await context.Discounts.ToListAsync();
                var remainingProducts = await context.Products.ToListAsync();

                Assert.Empty(remainingDiscounts);
                Assert.Empty(remainingProducts);
            }
        }
    }
}