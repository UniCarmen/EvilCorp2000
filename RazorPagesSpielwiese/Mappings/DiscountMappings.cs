using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Mappings
{
    public class DiscountMappings
    {
        public DiscountDTO DiscountToDiscountDTO (Discount discount)
        {
            return new DiscountDTO
            {
                DiscountId = discount.DiscountId,
                DiscountPercentage = discount.DiscountPercentage,
                EndDate = discount.EndDate,
                StartDate = discount.StartDate,
            };
        }
    }
}
