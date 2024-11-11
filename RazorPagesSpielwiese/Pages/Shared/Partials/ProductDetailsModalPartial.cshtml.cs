using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.Diagnostics.Eventing.Reader;

namespace RazorPagesSpielwiese.Pages.Shared.Partials
{
    public class ProductDetailsModalPartialModel
    {
        public ProductForInternalUseDTO Product { get; set; }

        public void OnGet()
        {
        }

    }
}
