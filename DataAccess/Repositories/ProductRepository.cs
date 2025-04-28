using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Shared.Utilities;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.ConstrainedExecution;
using static Shared.Utilities.Utilities;

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

        public async Task<List<Product>> GetHighlightProducts()
        {
            try
            {
                var random = new Random();

                var products = await _context.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    .AsNoTracking()
                    .AsSplitQuery()
                    //Producte mit aktivem Discount
                    .Select(p => new
                    {
                        Product = p,
                        ActiveDiscount = p.Discounts
                                .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                                .FirstOrDefault()
                    })
                    .Where(p => p.ActiveDiscount != null)
                    .Select(x => x.Product)
                    .ToListAsync();
                //zufällig 2 - versucht keinen Fehler bei Leerer Liste

                var rand2 = products
                    .OrderBy(_ => random.Next())
                    .Take(2)
                    .ToList();

                return rand2;

            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error while getting the products");
                throw;
            }
        }

        //public async Task<List<Product>> GetAllProductsAsync(ProductSortOrder? sortOrder = null, int? pageNumber = 1, int? pageSize = 10) 
        public async Task<ProductListReturn<Product>> GetAllProductsAsync(ProductSortOrder? sortOrder = null, int? pageNumber = 1, int? pageSize = 10)
        {
            try
            {
                pageNumber = (pageNumber.HasValue && pageNumber.Value > 0) ? pageNumber.Value : 1;
                pageSize = (pageSize.HasValue && pageSize.Value > 0) ? pageSize.Value : 10;
                sortOrder = sortOrder ?? ProductSortOrder.Default;

                IQueryable<Product> query = _context.Products
                    .Include(p => p.Categories)
                    .Include(p => p.Discounts)
                    .AsNoTracking()
                    .AsSplitQuery(); //INFO: teilt Abfrage in mehrere einfache SQL-Abfragen, verbessert Performance bei bielen Includes.

                //für Filter -> checken, searchTerm als Parameter, ich bräuchte auch Kategorie, Bewertung und sowas als Parameter... Suchtyp anlegen?
                //if (!string.IsNullOrEmpty(searchTerm))
                //{
                //    query = query.Where(p => p.ProductName.Contains(searchTerm));
                //}

                var productCount = query.Count();

                int maxPageCount = (int)Math.Ceiling((double)productCount / pageSize.Value);

                query = sortOrder switch
                {
                    ProductSortOrder.PriceAsc => query.OrderBy(p => p.ProductPrice),
                    ProductSortOrder.PriceDesc => query.OrderByDescending(p => p.ProductPrice),
                    ProductSortOrder.DiscountAsc => query
                        //nur Produkte mit aktivem Rabatt    
                        .Select(p => new
                        {
                            Product = p,
                            ActiveDiscount = p.Discounts
                                .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                                .FirstOrDefault()
                        })
                        //Produkte werden sortiert: mit aktivem Discount der Höhe nach, alle ohne DIscount am Ende der Liste mit double.MaxValue
                        .OrderBy(x => x.ActiveDiscount != null ? x.ActiveDiscount.DiscountPercentage : double.MaxValue)
                        .Select(x => x.Product),
                    ProductSortOrder.DiscountDesc => query
                        .Select(p => new
                        {
                            Product = p,
                            ActiveDiscount = p.Discounts
                                    .Where(d => d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now)
                                    .FirstOrDefault()
                        })
                            .OrderByDescending(x => x.ActiveDiscount != null ? x.ActiveDiscount.DiscountPercentage : double.MinValue)
                            .Select(x => x.Product),
                    ProductSortOrder.RatingDesc => query
                        .OrderByDescending(p => p.Rating != null ? p.Rating : double.MinValue),
                    ProductSortOrder.RatingAsc => query
                        .OrderBy(p => p.Rating != null ? p.Rating : double.MaxValue),
                    ProductSortOrder.NameAsc => query
                        .OrderBy(p => p.ProductName),
                    ProductSortOrder.NameDesc => query
                        .OrderByDescending(p => p.ProductName),
                    ProductSortOrder.StockAsc => query
                        .OrderBy(p => p.AmountOnStock),
                    ProductSortOrder.StockDesc => query
                        .OrderByDescending(p => p.AmountOnStock),
                    _ => query
                };

                // TODO1: PAGINATION -> paging typ anlegen, damit nicht zu viele parameter?

                query = query.OrderBy(p => p.ProductId)
                    .Skip((pageNumber.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

                var productList = await query.ToListAsync();


                


                return new ProductListReturn<Product?> 
                { 
                    ProductList = productList, 
                    ProductCount = productCount, 
                    MaxPageCount = maxPageCount
                };
                
                //return await query.ToListAsync();
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
            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

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
            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

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
            product = Utilities.ReturnValueOrThrowExceptionWhenNull(product, "Product is null.");
            
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
            productToBeUpdated = Utilities.ReturnValueOrThrowExceptionWhenNull(productToBeUpdated, "Product that should be updated is null");
            
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

                var errorMessage = "Product does not exist.";

                if (product == null)
                {
                    _logger.LogWarning(errorMessage);
                    throw new ArgumentNullException(nameof(product), errorMessage);
                }

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
            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

            var product = await GetProductById(productId);

            if (product == null)
            {
                _logger.LogWarning(errorMessage);
                throw new ArgumentNullException(nameof(product), errorMessage);
            }
            return product;
        }


        public async Task DeleteProductPicture(Guid productId)
        {
            try
            {
                productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, $"Invalid productId {productId}");

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
