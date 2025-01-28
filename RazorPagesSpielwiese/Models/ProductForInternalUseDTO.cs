namespace EvilCorp2000.Models
{
    public class ProductForInternalUseDTO //INFO Business Objekt
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public Guid ProductId { get; set; }
        public string? ProductPicture { get; set; }
        
        public decimal Price { get; set; }
        public List<DiscountDTO> Discounts { get; set; } = new List<DiscountDTO>();
        public double? Rating { get; set; }
        public int AmountOnStock { get; set; }
        //TODO: Status: Aktiv / nicht aktiv
    }
}
