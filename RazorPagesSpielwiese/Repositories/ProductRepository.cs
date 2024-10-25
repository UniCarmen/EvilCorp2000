using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EvilCorp2000ShopContext _context;

        public ProductRepository(EvilCorp2000ShopContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product?> GetProductById(int id)
        {
            if (id == 0) { throw new ArgumentNullException("id = 0, kein Eintrag"); }
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
    }
}
