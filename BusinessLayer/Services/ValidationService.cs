using BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    /// <summary>
    /// Validates, if ProductManagementProductDTO is valid:
    /// ProductName is Unique
    /// Price is greater than 0
    /// AmountOnStock isn't negative
    /// </summary>
    /// <returns>A Dictionary with validationErrors</returns>
    public static class ValidationService
    {
        public static Dictionary<string, string> ValidateProduct(ProductManagementProductDTO productToStore, bool nameIsUnique)
        {
            var validationErrors = new Dictionary<string, string>();

            if (!nameIsUnique)
            {
                validationErrors.Add("UniqueProductName", "Product name must be unique.");
            }

            if (productToStore.Price <= 0.0m)
            {
                validationErrors.Add("Price", "Price must be greater than 0.");
            }

            if (productToStore.AmountOnStock < 0)
            {
                validationErrors.Add("AmountOnStock", "Amount on stock cannot be negative.");
            }

            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(";", validationErrors));
            }
            return validationErrors;
        }

        //TODO 1: Warum gebe ich diese Fehler nicht zurück an die Oberfläche? Das mache ich mit denen oben doch auch
        /// <summary>
        /// Validates, if DiscountDTO is valid:
        /// Start and End Dates are not in the past
        /// End date comes after Start Date
        /// Discount dates don't overlap
        /// Discount percentage is greater than 0
        /// </summary>
        /// <returns>A Dictionary with validationErrors</returns>
        public static void ValidateDiscountAsync(DiscountDTO discount, List<DiscountDTO> discounts)
        {
            var validationErrors = new List<string>();

            if (discount.StartDate < DateTime.Today || discount.EndDate < DateTime.Today)
                validationErrors.Add("Start and End dates must not be in the past.");

            if (discount.StartDate >= discount.EndDate)
                validationErrors.Add("End Date must be after Start Date.");

            if (discount.DiscountPercentage <= 0)
                validationErrors.Add("Discount Percentage must be greater than 0.");

            if (discounts.Any(d => discount.StartDate < d.EndDate && discount.EndDate > d.StartDate))
            {
                validationErrors.Add("Discount overlaps with an existing discount.");
            }

            if (validationErrors.Any())
            {
                throw new ValidationException(string.Join(" ", validationErrors));
            }
        }
    }
}
