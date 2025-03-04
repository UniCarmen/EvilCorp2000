using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Shared.Utilities;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EvilCorp2000Context _context;
        private readonly ILogger<DiscountRepository> _logger;

        public DiscountRepository(EvilCorp2000Context context, ILogger<DiscountRepository>logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Discount>> GetDiscountsByProductId(Guid productId)
        {
            try
            {
                productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}.");
                return await _context.Discounts.Where(p => p.ProductId == productId).ToListAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Database error trying to get product with id {productId}");
                throw;
            }
        }

        public async Task<Discount?> GetCurrentDiscountByProductId(Guid productId)
        {
            try
            {
                productId = Utilities.ThrowExceptionWhenDefault(productId, $"Invalid productId {productId}.");
                //if (productId == Guid.Empty) { throw new ArgumentException("Invalid Guid"); }
                return await _context.Discounts.
                    Where(p =>
                    p.ProductId == productId &&
                    DateTime.UtcNow <= p.EndDate &&
                    DateTime.UtcNow >= p.StartDate
                    ).FirstOrDefaultAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error trying to get product with id {productId}");
                throw;
            }
        }

        public async Task AddDiscount(Discount discount)
        {
            try
            {
                discount = Utilities.ThrowExceptionWhenNull(discount, "Discount is null.");
                //if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error trying to save discount with id{discount.DiscountId}, starting date {discount.StartDate}, productId {discount.DiscountId}");
                throw;
            }
        }

        public async Task UpdateDiscounts(Product productFromDB, List<Discount> discounts)
        {
            try 
            {
                productFromDB = Utilities.ThrowExceptionWhenNull(productFromDB, "Product from DB is null.");
                
                //INFO: alle weg, die NICHT in der neuen DiscountListe vorhanden sind, kann bei vielen Disounts ineffizient werden
                //INFO: effektiver wäre die Verwendung eines HashSets (für Lookup optimiert)
                //INFO: verstehen wäre gut.
                //var updatedDiscountIds = discounts.Select(d => d.DiscountId).ToHashSet();

                //var discountsToRemove = productFromDB.Discounts
                //    .Where(d => !updatedDiscountIds.Contains(d.DiscountId))
                //    .ToList();

                var discountsToRemove = productFromDB.Discounts
                    .Where(d => !discounts.Any(ud => ud.DiscountId == d.DiscountId))
                    .ToList();

                foreach (var discount in discountsToRemove)
                {
                    productFromDB.Discounts.Remove(discount);
                }

                foreach (var updatedDiscount in discounts)
                {
                    var existingDiscount = productFromDB.Discounts
                        .FirstOrDefault(d => d.DiscountId == updatedDiscount.DiscountId);

                    if (existingDiscount == null)
                    {
                        // Neuer Discount
                        productFromDB.Discounts.Add(updatedDiscount);
                        //INFO: sagt dem EF, dass der Discount geadded werden soll, sonst wird er als modified getrackt und nicht gespeichert.
                        _context.Entry(updatedDiscount).State = EntityState.Added;
                    }
                    else
                    {
                        //TODO: Aktualisiere bestehenden Discount, falls Änderungen - momentan in der Oberfläche noch nicht implementiert
                        existingDiscount.StartDate = updatedDiscount.StartDate;
                        existingDiscount.EndDate = updatedDiscount.EndDate;
                        existingDiscount.DiscountPercentage = updatedDiscount.DiscountPercentage;
                    }

                    var a = _context.Entry(productFromDB);
                    var b = _context.ChangeTracker.Entries();
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error trying to update discounts of product with id{productFromDB.ProductId}");
                throw;
            }
        }

        public async Task DeleteDiscount(Discount discount)
        {
            try
            {
                discount = Utilities.ThrowExceptionWhenNull(discount, "Discount was null.");
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Successfully deleted {discount.DiscountId} with starting date {discount.StartDate} from product {discount.ProductId}.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error trying to delete discount of product with id{discount.ProductId}, productId {discount.ProductId}");
                throw;
            }
        }
    }
}
