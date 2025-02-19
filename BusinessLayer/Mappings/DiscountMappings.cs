using DataAccess.Entities;
using BusinessLayer.Models;

namespace BusinessLayer.Mappings
{
    public class DiscountMappings
    {
        public DiscountDTO DiscountToDiscountDTO(Discount discount)
        {
            if(discount == null)
            { throw new ArgumentNullException (nameof(discount)); }
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
            if(discountDTO == null)
            {
                throw new ArgumentNullException(nameof(discountDTO));
            }

            if(productId == Guid.Empty)
            {
                throw new ArgumentException("ProductId cannot be empty", nameof(productId));
            }
            
            Guid discountId = discountDTO.DiscountId;
            if (discountId == Guid.Empty)
            {
                discountId = Guid.NewGuid();
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
    }
}
