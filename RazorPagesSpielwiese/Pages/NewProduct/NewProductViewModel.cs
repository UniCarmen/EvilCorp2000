using System.ComponentModel.DataAnnotations;

namespace RazorPagesSpielwiese.Pages.NewProduct
{
    public class NewProductViewModel
    {
        [Required(ErrorMessage = "Productname required")]
        [Display(Name = "Productname")]
        [StringLength(100)]
        public string ProductName { get; set; } = "";

        [Display(Name = "Description")]
        public string Description { get; set; } = "";

        [Required(ErrorMessage = "Category required")]
        [Display(Name = "Category")]
        public List<Guid> CategoriyIdsFromSelect { get; set; } = [];

        [Display(Name = "ProductPicture")]
        public string? ProductPicture { get; set; }

        [Required(ErrorMessage = "Price required")]
        [Display(Name = "Price")]
        public decimal Price { get; set; }

        //[Display(Name = "")]
        //public double? Discount { get; set; }

        [Required(ErrorMessage = "Amount on Stock required")]
        [Display(Name = "Amount on Stock")]
        public int AmountOnStock { get; set; }
    
}
}
