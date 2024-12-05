using RazorPagesSpielwiese.Entities;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Mappings
{
    public class DiscountMappings
    {
        public DiscountDTO DiscountToDiscountDTO(Discount discount)
        {
            return new DiscountDTO
            {
                DiscountId = discount.DiscountId,
                DiscountPercentage = discount.DiscountPercentage,
                EndDate = discount.EndDate,
                StartDate = discount.StartDate,
            };
        }

        public Discount DiscountDTOToDiscount(DiscountDTO discountDTO, Guid productId)
        {
            Guid discountId = discountDTO.DiscountId;
            if (discountDTO.DiscountId == Guid.Empty)
            {
                discountDTO.DiscountId = Guid.NewGuid();
            }

            return new Discount
            {
                ProductId = productId,
                DiscountId = discountId,
                DiscountPercentage = discountDTO.DiscountPercentage,
                EndDate = discountDTO.EndDate,
                StartDate = discountDTO.StartDate,
            };
        }

        public DiscountDTO SetDiscountId(DiscountDTO discount)
        {
            discount.DiscountId = Guid.NewGuid();
            return discount;
        }

    }
}
