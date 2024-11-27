using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;
using System.Diagnostics.Eventing.Reader;

namespace RazorPagesSpielwiese.Pages.ProductManagement.Partials
{
    public class AlterProductModalPartialModel
    {
        //wurde mitgegeben und wird auch an Index zurückgegeben
        [BindProperty]
        public ProductForInternalUseDTO Product { get; set; }


        public List<CategoryDTO> Categories { get; set; }

        [BindProperty]
        public List<CategoryDTO> SelectedCategoryIds { get; set; }



        public async Task OnGet()
        {
        }

    }
}
