using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public class CategoryRepository
    {
        private readonly EvilCorp2000ShopContext _context;

        public CategoryRepository(EvilCorp2000ShopContext context)
        {
            _context = context;
        }

        public async Task<List<Discount>> GetAllCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        //Save

        //Update

        //Delete
    }
}
