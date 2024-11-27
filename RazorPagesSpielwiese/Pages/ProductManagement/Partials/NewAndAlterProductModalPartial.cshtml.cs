using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductValidations;

namespace RazorPagesSpielwiese.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel1
    {
        
        [BindProperty]
        //? hatte ich es optional wegen der automatischen Validierung - die nicht funktioniert - oder weil es evtl null sein könnte
        //jetzt könnte es nicht mehr null sein, da ich ein leeres Product mit einer Empty Guid mitgebe
        public ValidatedProduct ValidatedProduct { get; set; }

        //von Backing Code Index empfangen
        public List<CategoryDTO> Categories { get; set; }

        public async Task OnGet()
        {
        }

    }
}
