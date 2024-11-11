using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;
using System.Diagnostics.Eventing.Reader;

namespace RazorPagesSpielwiese.Pages.Shared.Partials
{
    public class AlterProductModalPartialModel
    {
        [BindProperty]
        public ProductForInternalUseDTO Product { get; set; }

        //public ProductToStoreDTO ProductToStore { get; set; }
        public List<CategoryDTO> Categories { get; set; }

        [BindProperty]
        public List<CategoryDTO> SelectedCategoryIds { get; set; }



        public async Task OnGet()
        {
        }

    }
}
