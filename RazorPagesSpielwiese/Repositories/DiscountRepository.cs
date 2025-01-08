using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesSpielwiese.DBContexts;
using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesSpielwiese.Repositories
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

        public async Task UpdateDiscount(Discount discount)
        {
            if (discount == null) { throw new ArgumentNullException(nameof(discount)); }
            _context.Discounts.Update(discount);
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
