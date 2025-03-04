using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public interface ICategoryRepository
    {
        Task DeleteCategory(Category productClassToDelete);
        Task<List<Category>> GetAllCategories();
        //Task<List<Category>> GetAllCategoriesTest();
        Task SaveNewCategory(Category productClass);
        List<Category> AttachCategoriesIfNeeded(List<Category> categories);
        Task UpdateCategories(Product productFromDB, List<Category> categories);
    }
}