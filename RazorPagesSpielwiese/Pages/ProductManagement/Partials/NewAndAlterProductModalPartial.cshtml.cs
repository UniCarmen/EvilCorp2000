using EvilCorp2000.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using static EvilCorp2000.Models.NewProductValidations;

namespace EvilCorp2000.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel
    {
        //private ValidatedProduct _validatedProduct;

        [BindProperty]
        public ValidatedProduct ValidatedProduct
        {
            get; set;
            //diese Prüfung habe ich jetzt im Backing Code der Hauptseite
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

        //[BindProperty]
        public bool DiscountOverlap { get; set; }

        [BindProperty]
        public string ValidatedProductJson { get; set; }

        [BindProperty]
        public string CategoryIdsJson { get; set; }


        public async Task OnGet()
        {
        }

    }
}
