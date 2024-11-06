using System.ComponentModel.DataAnnotations;

namespace RazorPagesSpielwiese.Models
{
    public class ProductToStoreDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public CategoryDTO Category { get; set; } = new CategoryDTO();
        public string? ProductPicture { get; set; }
        public decimal Price { get; set; }
        //public double? Discount { get; set; }
        public int AmountOnStock { get; set; }
    }
}