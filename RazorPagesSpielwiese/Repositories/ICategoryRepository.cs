using EvilCorp2000.Entities;

namespace EvilCorp2000.Repositories
{
    public interface ICategoryRepository
    {
        Task DeleteCategory(Category productClassToDelete);
        Task<List<Category>> GetAllCategories();
        Task SaveNewCategory(Category productClass);
        Task UpdateCategory(Category newProductClass);
        List<Category> AttachCategoriesIfNeeded(List<Category> categories);
    }
}