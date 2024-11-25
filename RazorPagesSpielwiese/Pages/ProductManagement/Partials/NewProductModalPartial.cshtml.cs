using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductViewModelValidations1;

namespace RazorPagesSpielwiese.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel1
    {
        [BindProperty]
        public NewProductViewModel1? ValidatedProduct { get; set; }

        //von Backing Code Index empfangen
        public List<CategoryDTO> Categories { get; set; }

        //f�r die R�ckkgabe an Backing Code Index
        [BindProperty]
        public List<CategoryDTO> SelectedCategoryIds { get; set; }



        public async Task OnGet()
        {
        }

    }
}
