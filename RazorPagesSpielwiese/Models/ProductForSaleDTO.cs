namespace EvilCorp2000.Models
{
    public class ProductForSaleDTO
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> Categories { get; set; } = new List<string>();
        public Guid ProductId { get; set; }
        public string? ProductPicture { get; set; }
        public decimal Price { get; set; }
        public double? Discount { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public double? Rating { get; set; }
        public int AmountOnStock { get; set; }
    }
}
