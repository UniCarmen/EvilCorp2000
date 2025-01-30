using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EvilCorp2000Context _context;
        private readonly ILogger<CategoryRepository> _logger;    

        public CategoryRepository(EvilCorp2000Context context, ILogger<CategoryRepository>logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            try
            {
                return await _context.Category.AsNoTracking().ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen der Categories");
                throw;
            }
        }

        public async Task SaveNewCategory(Category category)
        {
            try
            {
                if (category == null) { throw new ArgumentNullException("keine Productclass"); }
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Speichern der Category mit der ID {category.CategoryId}");
                throw;
            }
        }

        public async Task UpdateCategories(Product productFromDB, List<Category>categories)
        {
            try
            {
                var categoriesToRemove = productFromDB.Categories
                    .Where(c => !categories.Any(uc => uc.CategoryId == c.CategoryId))
                    .ToList();
                foreach (var category in categoriesToRemove)
                {
                    productFromDB.Categories.Remove(category);
                }
                var categoriesToAdd = categories
                    .Where(uc => !productFromDB.Categories.Any(c => c.CategoryId == uc.CategoryId))
                    .ToList();
                foreach (var category in categoriesToAdd)
                {
                    productFromDB.Categories.Add(category);
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Updaten der Categories des Produkts mit der ID {productFromDB.ProductId}");
                throw;
            }
        }

        public async Task DeleteCategory(Category categoryToDelete)
        {
            try
            {
                if (categoryToDelete == null) { throw new ArgumentNullException("keine Productclass"); }

                var oldClass = await _context.Category.FirstAsync(p => p.CategoryId == categoryToDelete.CategoryId);

                if (oldClass == null)
                {
                    throw new ArgumentNullException("keine alte Klasse vorhanden");
                }
                _context.Category.Remove(oldClass);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Löschen der Category mit der ID {categoryToDelete.CategoryId}");
                throw;
            }

        }

        public List<Category> AttachCategoriesIfNeeded(List<Category> categories)
        {
            var attachedCategories = new List<Category>();

            foreach (var category in categories)
            {
                var existingCategory = _context.Category
                    //keine erneute DB Abfrage, sondern Zugriff auf den ChangeTracker
                    .Local
                    .FirstOrDefault(c => c.CategoryId == category.CategoryId);

                if (existingCategory == null)
                {
                    // Hinzufügen, falls Category noch nicht in Liste
                    _context.Attach(category);
                    attachedCategories.Add(category);
                }
                else
                {
                    // Nur bereits vorhandene Categories in Liste
                    attachedCategories.Add(existingCategory);
                }
            }

            return attachedCategories;
        }
    }
}
