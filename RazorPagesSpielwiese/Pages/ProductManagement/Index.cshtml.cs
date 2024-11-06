using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;
using RazorPagesSpielwiese.Services;

namespace RazorPagesSpielwiese.Pages.ProductManagement
{
    public class ProductManagementModel : PageModel
    {
        
        private readonly IInternalProductManager _internalProductManager;
        private readonly ILogger<ProductManagementModel> _logger;

        //Produkte prop
        public List<ProductForInternalUseDTO> Products;
        //public ProductForInternalUseDTO Product { get; set; }

        public ProductManagementModel(IInternalProductManager internalProductManager, ILogger<ProductManagementModel> logger)
        {
            _internalProductManager = internalProductManager;
            _logger = logger;
        }


        public async Task OnGet()
        {
            try
            {
                //TODO:
                //ViewModel ProductToChange, mit gewissen Inhalten als Strings, UI Object
                //Model ProductToChangeDTO, Business Object

                //ANZEIGEN
                //Produkte = await _internalProductManager.
                //InterneProdukte abrufen, anzeigen als Liste, mit Discount Infos (Datum)

                Products = await _internalProductManager.GetProductsForInternalUse();



                //VERÄNDERN
                //sie verändern, //Möglichkeit, Bestand zu ändern, Beschreibung, etc.
            }
            catch (Exception ex) { _logger.LogError("Fehler beim Abrufen der Produkte: {0}", ex); }

        }


        
    }
}