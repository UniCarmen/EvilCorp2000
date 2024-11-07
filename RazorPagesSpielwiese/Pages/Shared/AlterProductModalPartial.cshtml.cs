using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;
using System.Diagnostics.Eventing.Reader;

namespace RazorPagesSpielwiese.Pages.Shared
{
    public class AlterProductModalPartialModel 
    {
        public ProductForInternalUseDTO Product { get; set; }

        public List<CategoryDTO> Categories { get; set; }

        //public AlterProductModalPartialModel (IInternalProductManager productManager)
        //{
        //    _productManager = productManager;
        //}

        public async Task OnGet()
        {
            //Categories = await _productManager.GetCategories();
        }

    }
}
