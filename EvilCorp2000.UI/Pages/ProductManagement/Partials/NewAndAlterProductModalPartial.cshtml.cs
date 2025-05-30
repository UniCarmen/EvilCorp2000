using BusinessLayer.Models;
using EvilCorp2000.UIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace EvilCorp2000.Pages.ProductManagement.Partials
{
    public class NewProductModalPartialModel
    {
        //private ValidatedProduct _validatedProduct;

        [BindProperty]
        public ValidatedProduct ValidatedProduct
        {
            get; set;
            //diese Pr�fung habe ich jetzt im Backing Code der Hauptseite
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

        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public string SortOrder { get; set; } = "Default";

        public List<CategoryDTO> Categories { get; set; }

        [BindProperty]
        public ValidatedDiscount? NewDiscount { get; set; }

        [BindProperty]
        public string DiscountsJson { get; set; }

        public bool DiscountOverlap { get; set; }

        [BindProperty]
        public string ValidatedProductJson { get; set; }

        [BindProperty]
        public string CategoryIdsJson { get; set; }

        //brauche ich nicht binden?
        //[BindProperty]
        [Display(Name = "Upload Produktbild")]
        //[BindProperty(SupportsGet = true)]
        public IFormFile? ImageFile { get; set; }


        public async Task OnGet()
        {
        }

    }
}
