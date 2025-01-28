using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        Task DeleteCategory(Category productClassToDelete);
        Task<List<Category>> GetAllCategories();
        Task SaveNewCategory(Category productClass);
        Task UpdateCategories(Product productFromDB, List<Category> categories);
        List<Category> AttachCategoriesIfNeeded(List<Category> categories);
    }
}