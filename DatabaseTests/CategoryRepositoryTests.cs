using DataAccess.DBContexts;
using DataAccess.Entities;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseTests
{
    public class CategoryTests
    {
        private EvilCorp2000Context CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EvilCorp2000Context>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new EvilCorp2000Context(options);
        }

        private static Category Createcategory1() { return new Category { CategoryId = Guid.NewGuid(), CategoryName = "Weapons" }; }
        private static Category Createcategory2() { return new Category { CategoryId = Guid.NewGuid(), CategoryName = "Armor" }; } 

        [Fact]
        public async Task GetAllCategories_ShouldReturnAllCategories()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var logger = NullLogger<CategoryRepository>.Instance;
            var repository = new CategoryRepository(context, logger);

            context.Category.AddRange(Createcategory1(), Createcategory2());
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllCategories();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CategoryName == "Weapons");
            Assert.Contains(result, c => c.CategoryName == "Armor");
        }


        [Fact]
        public async Task SaveNewCategory_ShouldAddCategoryToDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var logger = NullLogger<CategoryRepository>.Instance;
            var repository = new CategoryRepository(context, logger);

            // Act
            await repository.SaveNewCategory(Createcategory1());
            var result = await context.Category.FirstOrDefaultAsync(c => c.CategoryName == "Weapons");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Weapons", result.CategoryName);
        }

        [Fact]
        public async Task SaveNewCategory_ShouldThrowException_WhenCategoryIsNull()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var logger = NullLogger<CategoryRepository>.Instance;
            var repository = new CategoryRepository(context, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.SaveNewCategory(null));
        }


        [Fact]
        public async Task DeleteCategory_ShouldRemoveCategoryFromDatabase()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var logger = NullLogger<CategoryRepository>.Instance;
            var repository = new CategoryRepository(context, logger);

            var categoryToDelete = Createcategory1();
            context.Category.Add(categoryToDelete);
            await context.SaveChangesAsync();

            // Act
            await repository.DeleteCategory(categoryToDelete);
            var result = await context.Category.FirstOrDefaultAsync(c => c.CategoryId == categoryToDelete.CategoryId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteCategory_ShouldThrowException_WhenCategoryIsNull()
        {
            // Arrange
            using var context = CreateInMemoryDbContext();
            var logger = NullLogger<CategoryRepository>.Instance;
            var repository = new CategoryRepository(context, logger);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => repository.DeleteCategory(null));
        }
    }
}
