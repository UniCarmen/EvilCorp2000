using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics.Eventing.Reader;

namespace EvilCorp2000.Pages.ProductManagement.Partials
{
    public class ProductDetailsModalPartialModel
    {
        public ProductManagementProductDTO Product { get; set; }

        public void OnGet()
        {
        }

    }
}
