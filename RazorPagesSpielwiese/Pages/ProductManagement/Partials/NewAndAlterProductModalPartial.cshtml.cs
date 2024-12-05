using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using static RazorPagesSpielwiese.Models.NewProductValidations;

namespace RazorPagesSpielwiese.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel1
    {
        //private ValidatedProduct _validatedProduct;

        [BindProperty]        
        public ValidatedProduct ValidatedProduct
        { get;set;
            //get => _validatedProduct; set
            //{
            //    _validatedProduct = value;
            //    if (_validatedProduct != null)
            //    {
            //        var json = JsonSerializer.Serialize(_validatedProduct.Discounts);
            //        DiscountsJson = json;
            //    }
            //    else
            //        DiscountsJson = null;
            //}
        }

        //von Backing Code Index empfangen
        public List<CategoryDTO> Categories { get; set; }

        [BindProperty]
        public ValidatedDiscount? NewDiscount { get; set; }

        [BindProperty]
        public string DiscountsJson { get; set; }

        [BindProperty]
        public string ValidatedProductJson { get; set; }

        [BindProperty]
        public string CategoryIdsJson { get; set; }


        public async Task OnGet()
        {
        }

    }
}
