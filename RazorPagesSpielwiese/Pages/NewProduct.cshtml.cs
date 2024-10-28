using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesSpielwiese.Models;

namespace RazorPagesSpielwiese.Pages
{
    public class NewProductModel : PageModel
    {
        private readonly ILogger<NewProductModel> _logger;

        //bindet eingegebene Formulardaten an den Typen
        [BindProperty]
        public ProductToStoreDTO ProductToStore { get; set; }

        public NewProductModel(ILogger<NewProductModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            //get categorys from service
        }

        //IAction Result, um zur gleichen Seite redirecten zu können
        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Falls das Formular ungültig ist, bleibe auf der Seite und zeige Fehler an
                return Page();
            }

            // Hier könnte die Nachricht verarbeitet werden, z. B. durch Speichern in einer Datenbank
            // Oder durch das Versenden einer E-Mail etc.

            return RedirectToPage(); //oder Page()
            //später mal? auf eine andere Seiten Leiten
            //return RedirectToPage("Success");
        }
    }

}
