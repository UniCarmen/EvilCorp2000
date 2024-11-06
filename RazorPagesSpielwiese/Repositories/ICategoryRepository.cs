using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public interface ICategoryRepository
    {
        Task DeleteProductClass(Category productClassToDelete);
        Task<List<Category>> GetAllCategories();
        Task SaveNewCategory(Category productClass);
        Task UpdateCategory(Category newProductClass);
    }
}