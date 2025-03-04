using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Shared.Utilities;
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
                _logger.LogError(ex, $"Database error while getting the products");
                throw;
            }
        }

        //INFO: Product ist nullable, damit die Methode weiß, dass etwas nullable zurückgegeben werden kann
        //INFO: Der Aufrufer entscheidet, was passiert, wenn Null kommt -> Business-Entscheidung.
        public async Task<Product?> GetProductByIdWithCategoriesAnsdDiscounts(Guid productId)
        {
            productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

            try
            {
                var product = await _context.Products
                    //.AsNoTracking()
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    //.AsNoTracking()
                    .Where(p => p.ProductId == productId).FirstOrDefaultAsync();
                return product;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while getting product with id {productId}");
                throw;
            }
        }

        public async Task<Product?> GetProductById (Guid productId)
        {
            productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);

                return product;
            }

            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while getting product with id {productId}");
                throw;
            }
        }

        public async Task AddProduct(Product product)
        {
            product = Utilities.ThrowExceptionWhenNull(product, "Product is null.");
            
            try
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while adding product with id {product.ProductId}");
                throw;
            }
        }

        public async Task UpdateProduct(Product productToBeUpdated)
        {
            productToBeUpdated = Utilities.ThrowExceptionWhenNull(productToBeUpdated, "Product that should be updated is null");
            
            try
            {
                //INFO: falls das Product irgendwann untracked wurde, ansonsten kann es sein, dass EF Core die Änderung nicht durchführt
                //INFO: aktuell sollte es noch getrackt werden
                _context.Entry(productToBeUpdated).State = EntityState.Modified; 
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Database error while updating product with id {productToBeUpdated.ProductId}");
                throw;
            }
        }


        public async Task DeleteProduct(Guid productId)
        {
            try
            {
                var product = await GetProduct(productId, ($"Product does not exist. ProductId: {productId}"));

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully deleted {product.ProductName} with Id: {productId}.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting product with ID {productId}");
                throw;
            }
        }


        public async Task SaveProductPicture(Guid productId, string encodedPicture)
        {
            try
            {
                var product = await GetProduct(productId, ($"Product does not exist. ProductId: {productId}"));

                product.ProductPicture = encodedPicture;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while saving the picture for product with id {productId}");
                throw;
            }
        }

        public async Task<Product> GetProduct(Guid productId, string errorMessage)
        {
            productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

            var product = await GetProductById(productId);

            if (product == null)
            {
                _logger.LogWarning(errorMessage);
                throw new ArgumentNullException(errorMessage);
            }
            return product;
        }


        public async Task DeleteProductPicture(Guid productId)
        {
            try
            {
                productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

                var product = await GetProductById(productId);

                if (product == null)
                {
                    var errorMessage = "Product from database is null";
                    _logger.LogWarning(errorMessage);
                    throw new ArgumentNullException(errorMessage);
                }

                product.ProductPicture = null;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while deleting picture for product with id {productId}");
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
