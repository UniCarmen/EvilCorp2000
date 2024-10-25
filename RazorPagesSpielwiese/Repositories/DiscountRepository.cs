using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly EvilCorp2000ShopContext _context;

        public DiscountRepository(EvilCorp2000ShopContext context)
        {
            _context = context;
        }

        public async Task<List<Discount>> GetAllDiscountsAsync()
        {
            return await _context.Discounts.ToListAsync();
        }

        public async Task<List<Discount>> GetDiscountsByProductId(int productId)
        {
            if (productId == 0) { throw new ArgumentNullException("id = 0, kein Eintrag"); }
            return await _context.Discounts.Where(p => p.ProductId == productId).ToListAsync();
        }

        public async Task<Discount> GetCurrentDiscountByProductId(int productId)
        {
            if (productId == 0) { throw new ArgumentNullException("id = 0, kein Eintrag"); }
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
