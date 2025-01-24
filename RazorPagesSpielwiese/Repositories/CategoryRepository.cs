using EvilCorp2000.DBContexts;
using EvilCorp2000.Entities;
using EvilCorp2000.Models;
using Microsoft.EntityFrameworkCore;

namespace EvilCorp2000.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EvilCorp2000Context _context;

        public CategoryRepository(EvilCorp2000Context context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Category.AsNoTracking().ToListAsync();
        }

        public async Task SaveNewCategory(Category category)
        {
            if (category == null) { throw new ArgumentNullException("keine Productclass"); }
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategories(Product productFromDB, List<Category>categories)
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

        public async Task DeleteCategory(Category categoryToDelete)
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
