namespace RazorPagesSpielwiese.Models
{
    public class ProductForSaleDTO
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }        
        public int ProductId { get; set; }
        public string? ProductPicture { get; set; }
        public decimal Price { get; set; } 
        public double? Discount { get; set; }
        public decimal? DiscountedPrice { get; set; }
        public int? Rating { get; set; }
        public int AmountOnStock { get; set; }
    }
}
