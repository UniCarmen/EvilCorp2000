using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Pages.Shared
{
    public class ModalPartialModel 
    {
        public ProductForInternalUseDTO Product { get; set; } 
        
        public void OnGet()
        {
        }
    }
}
