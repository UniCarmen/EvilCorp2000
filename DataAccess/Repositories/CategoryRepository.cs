using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Utilities;
using static Shared.Utilities.Utilities;

namespace DataAccess.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly EvilCorp2000Context _context;
        private readonly ILogger<CategoryRepository> _logger;    
        
        //INFO: Mit LoggingFacotry kann man verschiedene Logger z.B. in Shared verwenden, lohnt sich aber gerade nicht
        //private readonly ILogger _logger;
        //private readonly ILoggerFactory _loggerFactory;

        public CategoryRepository(EvilCorp2000Context context, ILogger<CategoryRepository> logger /*ILoggerFactory loggerFactory*/)
        {
            _context = context;
            _logger = logger;
            //_loggerFactory = loggerFactory;
            //_logger = loggerFactory.CreateLogger<CategoryRepository>();
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
                ThrowExceptionWhenNull(category, $"Category with id is missing");
                //INFO: alt: if (category == null) { throw new ArgumentNullException("Category is missing"); }
                _context.Category.Add(category);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error when trying to save Category with id {category.CategoryId}");
                throw;
            }
        }


        public async Task UpdateCategories(Product productFromDB, List<Category> categories)
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

        //TODO noch nicht eingebaut, will ich das überhaupt? Es dürfen keine Cat gelöscht werden, die einem Product zugeordnet sind
        public async Task DeleteCategory(Category categoryToDelete)
        {
            try
            {
                categoryToDelete = ThrowExceptionWhenNull(categoryToDelete, $"Category to delete is missing");

                var oldCategory = await _context.Category.FirstOrDefaultAsync(p => p.CategoryId == categoryToDelete.CategoryId);

                if (oldCategory == null)
                {
                    string errorMessage = $"Category does not exist in database. {categoryToDelete.CategoryId}";
                    _logger.LogWarning(errorMessage);
                    throw new ArgumentNullException(errorMessage);
                }

                _context.Category.Remove(oldCategory);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully deleted {categoryToDelete.CategoryName} with id {categoryToDelete.CategoryId}.");
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


        //INFO: Möglichkeit try/catches auszulagern, lohnt sich aber nur für längere Try/Catches, da man durch den langen Aufruf, nichts gewinnt.

        //public async Task<List<Category>> GetAllCategoriesTest()
        //{
        //    return await TryCatchExecution(async () =>
        //        await _context.Category.AsNoTracking().ToListAsync(),
        //        "Datenbankfehler beim Abrufen der Categories");
        //}

        //public async Task<T> TryCatchExecution<T>(Func<Task<T>> operation, string errorMessage)
        //{
        //    try
        //    {
        //        return await operation();
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        _logger.LogError(ex, errorMessage);
        //        throw;
        //    }
        //}
    }
}
