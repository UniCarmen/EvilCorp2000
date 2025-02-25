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

        //TODO noch nicht eingebaut
        public async Task SaveNewCategory(Category category)
        {
            try
            {
                if (category == null) { throw new ArgumentNullException("Category is missing"); }
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Speichern der Category mit der ID {category.CategoryId}");
                throw;
            }
        }


        //TODO noch nicht eingebaut, will ich das überhaupt?
        public async Task DeleteCategory(Category categoryToDelete)
        {
            try
            {
                if (categoryToDelete == null) { throw new ArgumentNullException("keine Productclass"); }

                var oldClass = await _context.Category.FirstOrDefaultAsync(p => p.CategoryId == categoryToDelete.CategoryId);

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
            //INFO: Dict Abruf schneller
            //var localCategories = _context.Category.Local.ToDictionary(c => c.CategoryId);
            var attachedCategories = new List<Category>();

            foreach (var category in categories)
            {
                //INFO: Dicst schneller, als FirstOrDefault, verstehe ich aber noch nicht ganz
                //if (!localCategories.TryGetValue(category.CategoryId, out var existingCategory))
                var existingCategory = _context.Category
                    //INFO: keine erneute DB Abfrage, sondern Zugriff auf den ChangeTracker
                    .Local
                    .FirstOrDefault(c => c.CategoryId == category.CategoryId);

                if (existingCategory == null)
                {
                    //INFO: Hinzufügen, falls Category noch nicht in Liste
                    _context.Attach(category);
                    attachedCategories.Add(category);
                }
                else
                {
                    //INFO: Nur bereits vorhandene Categories in Liste
                    attachedCategories.Add(existingCategory);
                }
            }

            return attachedCategories;
        }
    }
}
