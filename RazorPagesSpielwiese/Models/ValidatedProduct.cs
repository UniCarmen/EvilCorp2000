using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductValidations;
//TODO: momentan wird nur die GUID Liste validert, benötige ich den Rest später für Grenzwerte?

namespace RazorPagesSpielwiese.Models
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

        //[Required(ErrorMessage = "Category required")] //- funktioniert nicht weil GuidList
        [GuidListValidation(ErrorMessage = "Please select at least one category.")]
        [Display(Name = "Category")]
        public List<Guid> SelectedCategoryIds { get; set; } = [];

        [Display(Name = "Product Picture")]
        public string? ProductPicture { get; set; }

        //[DecimalValidation(0.99, "Price required")] falls ich Einschränkungen hinzufügen will
        [Required(ErrorMessage = "Price required")]
        [Display(Name = "Price")]
        public decimal? Price { get; set; }

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
