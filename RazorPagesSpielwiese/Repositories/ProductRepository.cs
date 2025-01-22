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
            return await _context.Products.Include(p => p.Categories).Include(c => c.Categories).Include(p => p.Discounts).AsNoTracking().ToListAsync();
        }

        public async Task<Product?> GetProductByIdWithCategoriesAnsdDiscounts(Guid id)
        {
            if (id == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
            return await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.Discounts)
                .Where(p => p.ProductId == id).FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductById (Guid productId)
        {
            if (productId == Guid.Empty) { throw new ArgumentNullException(nameof(productId)); }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product == null) { throw new InvalidOperationException(nameof(product)); }

            return product;
        }

        public async Task AddProduct(Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProduct(Product productToStore, Product productFromDB)
        {
            if (productToStore == null)
            { throw new ArgumentNullException(nameof(productToStore)); }

            productFromDB.ProductName = productToStore.ProductName;
            productFromDB.ProductDescription = productToStore.ProductDescription;
            productFromDB.ProductPicture = productToStore.ProductPicture;
            productFromDB.ProductPrice = productToStore.ProductPrice;
            productFromDB.AmountOnStock = productToStore.AmountOnStock;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteProduct(Guid productId)
        {
            var product = await GetProductById(productId);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            var product = await GetProductById(productId);

            product.ProductPicture = encodedPicture;

            await _context.SaveChangesAsync();
        }


        public async Task DeleteProductPicture(Guid productId)
        {
            var product = await GetProductById(productId);

            product.ProductPicture = null;

            await _context.SaveChangesAsync();
        }


        public async Task<bool> IsProductNameUniqueAsync(string name)
        {
            return !await _context.Products.AnyAsync(p => p.ProductName == name);
        }
    }
}
