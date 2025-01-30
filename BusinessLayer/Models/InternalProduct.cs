using System.ComponentModel.DataAnnotations;

namespace BusinessLayer.Models
{
    //BL Layer Klasse des Products, wird aber auch in der UI verwendet - theoretisch neue Klasse für UI erstellen
    public class InternalProduct
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CategoryDTO> Categories { get; set; } = [];
        public List<DiscountDTO> Discounts { get; set; } = [];
        public string? ProductPicture { get; set; }

        public decimal Price { get; set; }
        //public double? Discount { get; set; }
        public int AmountOnStock { get; set; }
        public double? Rating { get; set; }
        //TODO: Status: Aktiv / nicht aktiv
    }
}