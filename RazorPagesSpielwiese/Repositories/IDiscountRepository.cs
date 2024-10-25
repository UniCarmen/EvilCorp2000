using RazorPagesSpielwiese.Entities;

namespace RazorPagesSpielwiese.Repositories
{
    public interface IDiscountRepository
    {
        Task AddDiscount(Discount discount);
        Task DeleteDiscount(Discount discount);
        Task<List<Discount>> GetAllDiscountsAsync();
        Task<List<Discount>> GetDiscountsByProductId(int productId);
        Task<Discount> GetCurrentDiscountByProductId(int productId);
        Task UpdateDiscount(Discount discount);
    }
}