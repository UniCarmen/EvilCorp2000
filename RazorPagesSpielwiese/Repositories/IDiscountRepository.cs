using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public interface IDiscountRepository
    {
        Task AddDiscount(Discount discount);
        Task DeleteDiscount(Discount discount);
        Task<List<Discount>> GetAllDiscountsAsync();
        Task<List<Discount>> GetDiscountsByProductId(Guid productId);
        Task<Discount> GetCurrentDiscountByProductId(Guid productId);
        Task UpdateDiscount(Discount discount);
    }
}