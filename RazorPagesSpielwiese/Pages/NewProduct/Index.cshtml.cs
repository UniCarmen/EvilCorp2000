using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;
using System.ComponentModel.DataAnnotations;
using static RazorPagesSpielwiese.Models.NewProductValidations;

namespace RazorPagesSpielwiese.Pages.NewProduct
{
    public class NewProductModel : PageModel
    {
        //[BindProperty]
        //[Required(ErrorMessage = "Productname required")]
        //[Display(Name = "Productname")]
        //[StringLength(100)]
        //public string ValidatedProductName { get; set; } = "";


        //[BindProperty]
        //[Display(Name = "Description")]
        //public string? ValidatedDescription { get; set; } = "";

        //[BindProperty]
        //[GuidListValidation(ErrorMessage = "Please select at least one category.")]
        //[Display(Name = "Category")]
        //public List<Guid> ValidatedSelectedCategoryIds { get; set; } = [];

        //[BindProperty]
        //[Display(Name = "ProductPicture")]
        //public string? ValidatedProductPicture { get; set; }


        //[BindProperty]
        //[DecimalValidation(0.99, "Price required")]
        //[Display(Name = "Price")]
        //public decimal? ValidatedPrice { get; set; }

        ////[Display(Name = "")]
        ////public double? Discount { get; set; }

        //[BindProperty]
        //[IntValidation(ErrorMessage = "Amount on Stock required")]
        //[Display(Name = "Amount on Stock")]
        //public int? ValidatedAmountOnStock { get; set; }










        private readonly ILogger<NewProductModel> _logger;
        private readonly IInternalProductManager _internalProductManager;

        [BindProperty]
        //Verwendung von ViewModel, damit es bei der Validierung des ModelStates keine Probleme gibt, da ich den Wert für die Category erst herausfischen muss
        public NewProductViewModel? NewProduct { get; set; }
        public List<CategoryDTO> Categories { get; set; } = [];
        public List<SelectListItem> CategoriesForSelect { set; get; }


        public NewProductModel(ILogger<NewProductModel> logger, IInternalProductManager internalProductManager)
        {
            _logger = logger;
            _internalProductManager = internalProductManager;
        }

        public async Task OnGet()
        {
            await InitializeCategoriesAsync();
        }

        //IActionResult, um zur gleichen Seite redirecten zu können
        public async Task<IActionResult> OnPost()
        {
            try
            {
                //if (!HttpContext.Request.IsHttps)
                //{
                //    //Erzwinge HTTPS-Umleitung, falls die Anfrage nicht über HTTPS erfolgt
                //    //z.B. für Registrations, Logins...
                //    var httpsUrl = $"https://{HttpContext.Request.Host}{HttpContext.Request.Path}{HttpContext.Request.QueryString}";
                //    return Redirect(httpsUrl);
                //}

                var descriptionErrors = ModelState["NewProduct.Description"]?.Errors;
                if (descriptionErrors != null && descriptionErrors.Any())
                {
                    foreach (var error in descriptionErrors)
                    {
                        Console.WriteLine($"Description Error: {error.ErrorMessage}");
                    }
                }

                if (!ModelState.IsValid)
                {
                    // Falls das Formular ungültig ist, bleibe auf der Seite und zeige Fehler an
                    await InitializeCategoriesAsync();
                    return Page();
                }

                var categories = await _internalProductManager.GetCategories();

                int selectNumber;


                var category = categories.FindAll(c => NewProduct.CategoriyIdsFromSelect.Contains(c.CategoryId));

                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
                }

                if (NewProduct.Price == null)
                {
                    throw new ArgumentNullException(nameof(NewProduct.Price));
                }

                if (NewProduct.AmountOnStock == null)
                {
                    throw new ArgumentNullException(nameof(NewProduct.AmountOnStock));
                }

                var productToStore = new ProductToStoreDTO
                {
                    Price = NewProduct.Price.Value,
                    ProductName = NewProduct.ProductName,
                    AmountOnStock = NewProduct.AmountOnStock.Value,
                    Categories = category,
                    Description = NewProduct.Description,
                    ProductPicture = NewProduct.ProductPicture,
                    //TODO: noch füllen bzw von Oberfläche übergeben
                    Discounts = []
                };


                await _internalProductManager.SaveProductToStore(productToStore);

                //TempData, um in der Oberfläche anzuzeigen, dass der Speichervorgang erfolgreich gelaufen ist / Artikel erfolgreich gespeichert wurde
                TempData["SuccessMessage"] = "Artikel wurde erfolgreich gespeichert.";

                return RedirectToPage(); //oder Page()
                                         //später mal? auf eine andere Seiten Leiten
                                         //return RedirectToPage("Success");
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexpected Error: {0}", ex);
                return Page();
            }
        }

        private async Task InitializeCategoriesAsync()
        {
            try
            {
                Categories = await _internalProductManager.GetCategories();
                CategoriesForSelect = Categories
                    .Select((c, i) => new SelectListItem { Value = i.ToString("D"), Text = c.CategoryName })
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting the categories from the database: {0}", ex);
            }
        }
    }

}
