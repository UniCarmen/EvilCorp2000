using EvilCorp2000.DBContexts;
using EvilCorp2000.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace EvilCorp2000.Repositories
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
            { throw new ArgumentNullException(nameof(productEntity)); }

            //TODO: warum genau habe ich das nochmal so gemacht und nicht mit Update???
            //löschen des alten products: verbindungen in join tabellen werden automatisch gelöscht
            //das soll nicht sein. Das Product soll nur geupdated werden, evtl auch mit veränderten relationen, wenn z.B Categorien oder Discounts gelöscht oder hinzugefügt wurden.

            //die methode könnte leistungseinbußen haben, da ich kein update mache, sprich die Cat und Disc Tabellen manuell ändere, das geladene ProductIdentity ändere und dann speichere
            await DeleteProduct(productEntity.ProductId);

            await AddProduct(productToStore);
        }

        //TODO: sicherstellen, dass ein Produkt samt evtl. vorhandenen Discounts übergeben wird
        public async Task DeleteProduct(Guid productId)
        {
            if (productId == Guid.Empty) { throw new ArgumentNullException(nameof(productId)); }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) { throw new ArgumentNullException(nameof(product)); }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsProductNameUniqueAsync(string name)
        {
            return !await _context.Products.AnyAsync(p => p.ProductName == name);
        }
    }
}
