using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
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