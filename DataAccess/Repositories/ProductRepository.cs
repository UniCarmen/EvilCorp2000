using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;

namespace DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly EvilCorp2000Context _context;
        private readonly ILogger<ProductRepository> _logger;

        public ProductRepository(EvilCorp2000Context context, ILogger<ProductRepository>logger)
        {
            _context = context;
            //Logging in DAL, da Db-Fehler nicht explizit an die UI weitergeben werden, bzw. nur ungenau
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                //_logger.LogError("Test-Log: Fehler für Repo-Logging.");
                return await _context.Products.Include(p => p.Categories).Include(c => c.Categories).Include(p => p.Discounts).AsNoTracking().ToListAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen der Produkte");
                throw;
            }
        }

        public async Task<Product?> GetProductByIdWithCategoriesAnsdDiscounts(Guid id)
        {
            if (id == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
            try
            {
                return await _context.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    .Where(p => p.ProductId == id).FirstOrDefaultAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen des Produkts mit der ID {id}");
                throw;
            }
        }

        public async Task<Product> GetProductById (Guid productId)
        {
            if (productId == Guid.Empty) { throw new ArgumentNullException(nameof(productId)); }

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

                if (product == null) { throw new InvalidOperationException(nameof(product)); }

                return product;
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen des Produkts mit der ID {productId}");
                throw;
            }
        }

        public async Task AddProduct(Product product)
        {
            if (product == null) { throw new ArgumentNullException(nameof(product)); }
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Hinzufügen des Produkts {product.ProductName}");
                throw;
            }
        }

        public async Task UpdateProduct(Product productToStore, Product productFromDB)
        {
            if (productToStore == null || productFromDB == null)
            { throw new ArgumentNullException(nameof(productToStore)); }
            try
            { 
                productFromDB.ProductName = productToStore.ProductName;
                productFromDB.ProductDescription = productToStore.ProductDescription;
                productFromDB.ProductPicture = productToStore.ProductPicture;
                productFromDB.ProductPrice = productToStore.ProductPrice;
                productFromDB.AmountOnStock = productToStore.AmountOnStock;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Datenbankfehler beim Updaten des Produkts mit ID {productFromDB.ProductId}");
                throw;
            }
        }


        public async Task DeleteProduct(Guid productId)
        {
            try
            {
                var product = await GetProductById(productId);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Löschen des Produktes mit ID {productId}");
                throw;
            }
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            try
            { 
                var product = await GetProductById(productId);

                product.ProductPicture = encodedPicture;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Speichern des Bilder für das Produkt mit ID {productId}");
                throw;
            }
        }


        public async Task DeleteProductPicture(Guid productId)
        {
            try
            {
                var product = await GetProductById(productId);

                product.ProductPicture = null;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Löschen des Bildes für das Produkts mit ID {productId}");
                throw;
            }
        }


        public async Task<bool> IsProductNameUniqueAsync(string name, Guid id)
        {
            if (Guid.Empty == id)
            {
                return !await _context.Products.AnyAsync(p => p.ProductName == name);
            }

            //wenn nicht Empty, dann müssen alle geprüft werden, die nicht die gleiche ID haben, wie die das producttoStore
            //ist für das ändern des products, das soll ja auch keine doppelten Namen produzieren, aber auch keinen Fehler werfen, wenn ich das Produkt mit gleichbleibenden 
            //Namen speichere
            //var a = _context.Products.ToList();
            //var p = a.Where(p => p.ProductId != id);
            //var b = !p.Any(p => p.ProductName == name);
            //return
            //    b;
            return !await _context.Products.Where(p => p.ProductId != id).AnyAsync(p => p.ProductName == name);
            
        }

    }
}
