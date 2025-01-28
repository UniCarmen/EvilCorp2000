using DataAccess.DBContexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EvilCorp2000Context _context;

        public DiscountRepository(EvilCorp2000Context context)
        {
            _context = context;
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<List<Discount>> GetDiscountsByProductId(Guid productId)
        {
            if (productId == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
            return await _context.Discounts.Where(p => p.ProductId == productId).ToListAsync();
        }

        public async Task<Discount> GetCurrentDiscountByProductId(Guid productId)
        {
            if (productId == Guid.Empty) { throw new ArgumentNullException("Invalid Guid"); }
            return await _context.Discounts.
                Where(p =>
                p.ProductId == productId &&
                DateTime.UtcNow <= p.EndDate &&
                DateTime.UtcNow >= p.StartDate
                ).FirstOrDefaultAsync();
        }

        public async Task AddDiscount(Discount discount)
        {
            if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
            _context.Discounts.Add(discount);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDiscounts(Product productFromDB, List<Discount> discounts)
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
                    _context.Entry(updatedDiscount).State=EntityState.Added;
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

        public async Task DeleteDiscount(Discount discount)
        {
            if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
        }


    }
}
