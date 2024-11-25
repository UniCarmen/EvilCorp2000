using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductViewModelValidations1;

namespace RazorPagesSpielwiese.Models
{
    public class NewProductViewModel1
    {
        [Required(ErrorMessage = "Productname required")]
        [Display(Name = "Productname")]
        [StringLength(100)]
        public string ProductName { get; set; } = "";


        [Display(Name = "Description")]
        public string? Description { get; set; } = "";

        //[Required(ErrorMessage = "Category required")] - funktioniert nicht weil GuidList
        //[GuidListValidation(ErrorMessage = "Please select at least one category.")]
        //[Display(Name = "Category")]
        public List<Guid> SelectedCategoryIds { get; set; } = [];

        [Display(Name = "ProductPicture")]
        public string? ProductPicture { get; set; }


        //[DecimalValidation(0.99, "Price required")]
        [Required(ErrorMessage = "Price required")]
        [Display(Name = "Price")]
        public decimal? Price { get; set; }

        //[Display(Name = "")]
        //public double? Discount { get; set; }

        //[IntValidation(ErrorMessage = "Amount on Stock required")]
        [Required(ErrorMessage = "Amount on Stock required")]
        [Display(Name = "Amount on Stock")]
        public int? AmountOnStock { get; set; }
    }
}
