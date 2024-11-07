using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.Diagnostics.Eventing.Reader;

namespace RazorPagesSpielwiese.Pages.Shared
{
    public class ModalPartialModel 
    {
        public ProductForInternalUseDTO Product { get; set; } 
        public bool IsEditingMode { get; set; }

        public void OnGet()
        {
        }

    }
}
