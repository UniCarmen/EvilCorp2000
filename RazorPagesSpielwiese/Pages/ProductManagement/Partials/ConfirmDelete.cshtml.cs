using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvilCorp2000.Pages.ProductManagement.Partials
{
    public class ConfirmDeleteModel
    {
        public Guid SelectedProductId { get; set; }
        public string ProductName { get; set; }

        public void OnGet()
        {
        }

    }
}
