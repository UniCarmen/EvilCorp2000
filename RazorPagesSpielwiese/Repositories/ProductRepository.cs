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
            return await _context.Products.Include(p => p.Categories).Include(c => c.Categories).ToListAsync();
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

        public async Task UpdateProduct(Product productToStore)
        {
            if (productToStore == null) 
            { throw new ArgumentNullException(nameof(productToStore)); }
            
            var productEntity = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productToStore.ProductId);

            if (productEntity == null)
            { throw new ArgumentNullException(nameof(productEntity));}

            //löschen des alten products: verbindungen in join tabellen werden automatisch gelöscht
            //die methode könnte leistungseinbußen haben, da ich kein update mache, sprich die Cat und Disc Tabellen manuell ändere, das geladene ProductIdentity ändere und dann speichere
            await DeleteProduct(productEntity);

            await _context.SaveChangesAsync();

            await AddProduct(productToStore);

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
