using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages.NewProduct
{
    public class NewProductModel : PageModel
    {
        private readonly ILogger<NewProductModel> _logger;
        private readonly IInternalProductManager _internalProductManager;

        [BindProperty]
        //Verwendung von ViewModel, damit es bei der Validierung des ModelStates keine Probleme gibt, da ich den Wert für die Category erst herausfischen muss
        public NewProductViewModel NewProduct { get; set; }
        public List<CategoryDTO> Categories { get; set; }
        public List<SelectListItem> CategoriesForSelect { set; get; }


        public NewProductModel(ILogger<NewProductModel> logger, IInternalProductManager internalProductManager)
        {
            _logger = logger;
            _internalProductManager = internalProductManager;
        }

        public async Task OnGet()
        {

            try
            {
                Categories = await _internalProductManager.GetCategories();
                CategoriesForSelect = Categories
                    .Select((c, i) => new SelectListItem { Value = i/*+1*/.ToString("D"), Text = c.CategoryName })
                    .ToList();
            }
            catch (Exception ex) { _logger.LogError("Error getting the categories from the database: {0}", ex); }
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



                if (!ModelState.IsValid)
                {
                    // Falls das Formular ungültig ist, bleibe auf der Seite und zeige Fehler an
                    return Page();
                }

                var categories = await _internalProductManager.GetCategories();

                int selectNumber;


                var category = categories.FindAll(c => NewProduct.CategoriyIdsFromSelect.Contains(c.CategoryId));

                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
                }

                var productToStore = new ProductToStoreDTO
                {
                    Price = NewProduct.Price,
                    ProductName = NewProduct.ProductName,
                    AmountOnStock = NewProduct.AmountOnStock,
                    Categories = category,
                    Description = NewProduct.Description,
                    ProductPicture = NewProduct.ProductPicture,
                    //TODO: noch füllen bzw von Oberfläche übergeben
                    Discounts = []
                };


                await _internalProductManager.SaveProductToStore(productToStore);
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
    }

}
