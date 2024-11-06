using Microsoft.EntityFrameworkCore;
using RazorPagesSpielwiese.DBContexts;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
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
            return await _context.Category.ToListAsync();
        }

        public async Task SaveNewCategory(Category category)
        {
            if (category == null) { throw new ArgumentNullException("keine Productclass"); }
            _context.Category.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCategory(Category newCategory)
        {
            if (newCategory == null) { throw new ArgumentNullException("keine Productclass"); }

            var oldCategory = await _context.Category.FirstAsync(p => p.CategoryId == newCategory.CategoryId);

            if (oldCategory == null)
            {
                throw new ArgumentNullException("keine alte Klasse vorhanden");
            }
            _context.Category.Remove(oldCategory);
            _context.Add(newCategory);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductClass(Category categoryToDelete)
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
    }
}
