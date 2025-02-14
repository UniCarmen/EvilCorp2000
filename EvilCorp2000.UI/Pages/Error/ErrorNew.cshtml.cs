using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvilCorp2000.Pages.Error
{
    public class ErrorNewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public int? StatusCode { get; set; }

        public void OnGet(int? code)
        {
            StatusCode = code ?? 500;
        }
    }
}
