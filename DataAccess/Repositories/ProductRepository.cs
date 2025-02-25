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
            //INFO: Logging in DAL, da Db-Fehler nicht explizit an die UI weitergeben werden, bzw. nur ungenau
            _logger = logger;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            try
            {
                return await _context.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    .AsNoTracking()
                    .ToListAsync();
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
                var product = await _context.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    .Where(p => p.ProductId == id).FirstOrDefaultAsync();
                if (product == null) { throw new KeyNotFoundException(nameof(product)); }
                return product;
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

        public async Task UpdateProduct(Product productToBeUpdated)
        {
            if (productToBeUpdated == null)
            { throw new ArgumentNullException(nameof(productToBeUpdated)); }
            try
            {
                //INFO: falls das Product irgendwann untracked wurde, ansonsten kann es sein, dass EF Core die Änderung nicht durchführt
                //INFO: aktuell sollte es noch getrackt werden
                _context.Entry(productToBeUpdated).State = EntityState.Modified; 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Datenbankfehler beim Updaten des Produkts mit ID {productToBeUpdated.ProductId}");
                throw;
            }
        }


        public async Task DeleteProduct(Guid productId)
        {
            try
            {
                if (productId.Equals(Guid.Empty))
                { throw new ArgumentNullException(nameof(productId)); }

                var product = await GetProductById(productId);
                if (product == null)
                {
                    _logger.LogWarning($"Attempted to delete a product that does not exist. ProductId: {productId}");
                    throw new InvalidOperationException(nameof(product));
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully deleted {product.ProductName} with Id: {productId}.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error when deleting product with ID {productId}");
                throw;
            }
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            try
            { 
                //TODO1 doppelter Code wit bei DeleteProductPicture
                if(productId.Equals(Guid.Empty))
                { throw new ArgumentNullException(nameof(productId)); }

                var product = await GetProductById(productId);

                if(product == null)
                { throw new InvalidOperationException(nameof(product)); }

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
                if (productId.Equals(Guid.Empty))
                { throw new ArgumentNullException(nameof(productId)); }

                var product = await GetProductById(productId);

                if (product == null)
                { throw new InvalidOperationException(nameof(product)); }

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
            if (string.IsNullOrEmpty(name))
            { throw new ArgumentNullException(nameof(name)); }

            //INFO: Wenn Name schon vorhanden und es handelt sich um anderes Produkt = !true = nicht Unique
            //INFO: Wenn Name schon vorhanden und es handelt sich um dasselbe Produkt = !false = Unique
            return !await _context.Products.AnyAsync(p => p.ProductName == name && p.ProductId != id);

            //Dasselbe wie:
            //if (Guid.Empty == id)
            //{
            //    return !await _context.Products.AnyAsync(p => p.ProductName == name);
            //}
            //return !await _context.Products.Where(p => p.ProductId != id).AnyAsync(p => p.ProductName == name);
            
        }

    }
}
