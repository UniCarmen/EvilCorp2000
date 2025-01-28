using System.ComponentModel.DataAnnotations;
using static EvilCorp2000.Models.NewProductValidations;

namespace EvilCorp2000.Models
{
    public class ValidatedProduct
    {
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Product Name required")]
        [Display(Name = "Product Name")]
        [StringLength(100)]
        public string ProductName { get; set; } = "";


        [Display(Name = "Description")]
        public string? Description { get; set; } = "";

        //[Required(ErrorMessage = "Category required")] //- funktioniert nicht bei GuidList
        [GuidListValidation(ErrorMessage = "Please select at least one category.")]
        [Display(Name = "Category")]
        public List<Guid> SelectedCategoryIds { get; set; } = [];

        public string? ProductPicture { get; set; }

        [Required(ErrorMessage = "Price required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be at least 0.")]
        [Display(Name = "Price")]
        public decimal? Price { get; set; }

        [Required(ErrorMessage = "Amount on Stock required")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount on stock cannot be negative.")]
        [Display(Name = "Amount on Stock")]
        public int? AmountOnStock { get; set; }

        [Display(Name = "Discounts")]
        public List<DiscountDTO> Discounts { get; set; } = new List<DiscountDTO>();


        //TODO: Status: Aktiv / nicht aktiv??
    }
}
