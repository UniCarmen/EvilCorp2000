using RazorPagesSpielwiese.Entities;
using System.ComponentModel.DataAnnotations;

namespace RazorPagesSpielwiese.Models
{
    public class DiscountDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double DiscountPercentage { get; set; }
        public Guid DiscountId { get; set; }
    }
}
