using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesSpielwiese.DBContexts;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EvilCorp2000Context _context;

        public ProductRepository(EvilCorp2000Context context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {            
            return await _context.Products.Include(p => p.ProductCategoryMappings).ThenInclude(pcm => pcm.Category).ToListAsync();
        }

        public async Task<Product?> GetProductById(Guid id)
        {
            if (id == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
            return await _context.Products.Where(p => p.ProductId == id).FirstOrDefaultAsync();
        }

        public async Task AddProduct(Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProduct(Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Category>> GetCategoriesForProduct (Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            return await _context.ProductCategoryMappings
                .Where(pcm => pcm.ProductId == product.ProductId)
                .Select(pcm => pcm.Category)
                .ToListAsync();
        }
    }
}
