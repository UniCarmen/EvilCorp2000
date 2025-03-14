using DataAccess.Entities;
using BusinessLayer.Models;
using Shared.Utilities;

namespace BusinessLayer.Mappings
{
    public class DiscountMappings
    {
        public DiscountDTO DiscountToDiscountDTO(Discount discount)
        {
            discount = Utilities.ReturnValueOrThrowExceptionWhenNull(discount, "Discount is null.");
            
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
            discountDTO = Utilities.ReturnValueOrThrowExceptionWhenNull(discountDTO, "Discount is null.");

            productId = Utilities.ReturnValueOrThrowExceptionWhenDefault(productId, "ProductId cannot be empty.");

            var discountId = discountDTO.DiscountId == Guid.Empty ? Guid.NewGuid() : discountDTO.DiscountId;

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
