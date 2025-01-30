using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
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

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            try
            {
                return await _context.Discounts.ToListAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen der Discounts");
                throw;
            }
        }

        public async Task<List<Discount>> GetDiscountsByProductId(Guid productId)
        {
            try
            {
                if (productId == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
                return await _context.Discounts.Where(p => p.ProductId == productId).ToListAsync();
            }
            catch (DbUpdateException ex)
{
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen des Discounts des Produkts mit der ID {productId}");
                throw;
            }
        }

        public async Task<Discount> GetCurrentDiscountByProductId(Guid productId)
        {
            try
            {
                if (productId == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
                return await _context.Discounts.
                    Where(p =>
                    p.ProductId == productId &&
                    DateTime.UtcNow <= p.EndDate &&
                    DateTime.UtcNow >= p.StartDate
                    ).FirstOrDefaultAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Abrufen des Discounts des Produkts mit der ID {productId}");
                throw;
            }
        }

        public async Task AddDiscount(Discount discount)
        {
            try
            {
                if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Speichern des Discounts mit dem Startdatum {discount.StartDate}");
                throw;
            }
        }

        public async Task UpdateDiscounts(Product productFromDB, List<Discount> discounts)
        {
            try 
            {
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
                        //sagt dem EF, dass der Discount geadded werden soll, sonst wird er als modified getrackt und nicht gespeichert.
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
                _logger.LogError(ex, $"Datenbankfehler beim Updaten der Discounts des Produkts mit der ID {productFromDB.ProductId}");
                throw;
            }
        }

        public async Task DeleteDiscount(Discount discount)
        {
            try
            {
                if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Datenbankfehler beim Löschen des Discounts mit der ID {discount.ProductId}");
                throw;
            }
        }


    }
}
