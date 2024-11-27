using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductValidations;

namespace RazorPagesSpielwiese.Models
{
    public class ValidatedProduct
    {
        //TODO: Id als unsichtbares Feld in der View füllen über die Hauptseite.
        //kann Null sein, muss aber nicht. Die Id wird dann über das ValidatedProduct zurückgegeben
        //so kann festgestellt werden, ob es sich um ein verändertes Product oder ein neues Product handelt
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Product Name required")]
        [Display(Name = "Product Name")]
        [StringLength(100)]
        public string ProductName { get; set; } = "";


        [Display(Name = "Description")]
        public string? Description { get; set; } = "";

        //[Required(ErrorMessage = "Category required")] //- funktioniert nicht weil GuidList
        [GuidListValidation(ErrorMessage = "Please select at least one category.")]
        [Display(Name = "Category")]
        public List<Guid> SelectedCategoryIds { get; set; } = [];

        [Display(Name = "Product Picture")]
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

        //TODO: Neu anlegen: ValidatedDiscount
        //public DateTime StartDate { get; set; } --> mind heutiges Datum
        //public DateTime EndDate { get; set; }
        //public double DiscountPercentage { get; set; } --> nicht größer als 80
        //keine ID, wird entweder beim Speichern von altem Discount übernommen oder erzeugt

        //TODO: ...später Abfrage möglich, bei einem längeren Zeitraum als... 2 Wochen?

        [Display(Name = "Discounts")]
        public List<DiscountDTO> Discounts { get; set; } = new List<DiscountDTO>();


        //TODO: Status: Aktiv / nicht aktiv??
    }
}
