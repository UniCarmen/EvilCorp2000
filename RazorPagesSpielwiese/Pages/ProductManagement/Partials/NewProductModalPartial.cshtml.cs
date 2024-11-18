using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductViewModelValidations1;

namespace RazorPagesSpielwiese.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel1
    {
        //[BindProperty]
        //[Required(ErrorMessage = "Productname required")]
        //[Display(Name = "Productname")]
        //[StringLength(100)]
        //public string ValidatedProductName { get; set; } = "";


        //[BindProperty]
        //[Display(Name = "Description")]
        //public string? ValidatedDescription { get; set; } = "";

        //[BindProperty]
        //[GuidListValidation(ErrorMessage = "Please select at least one category.")]
        //[Display(Name = "Category")]
        //public List<Guid> ValidatedSelectedCategoryIds { get; set; } = [];

        //[BindProperty]
        //[Display(Name = "ProductPicture")]
        //public string? ValidatedProductPicture { get; set; }


        //[BindProperty]
        //[DecimalValidation(0.99, "Price required")]
        //[Display(Name = "Price")]
        //public decimal? ValidatedPrice { get; set; }

        ////[Display(Name = "")]
        ////public double? Discount { get; set; }

        //[BindProperty]
        //[IntValidation(ErrorMessage = "Amount on Stock required")]
        //[Display(Name = "Amount on Stock")]
        //public int? ValidatedAmountOnStock { get; set; }







        //für die Rückkgabe an Backing Code Index
        //[BindProperty]
        //public ProductForInternalUseDTO Product { get; set; }

        //geht wohl nicht??
        [BindProperty]
        public NewProductViewModel1 ValidatedProduct { get; set; }

        //von Backing Code Index empfangen
        public List<CategoryDTO> Categories { get; set; }

        //für die Rückkgabe an Backing Code Index
        [BindProperty]
        public List<CategoryDTO> SelectedCategoryIds { get; set; }



        public async Task OnGet()
        {
        }

    }
}
