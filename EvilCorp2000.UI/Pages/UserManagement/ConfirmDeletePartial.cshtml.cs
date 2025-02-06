using BusinessLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EvilCorp2000.Pages.UserManagement
{
    public class ConfirmDeletePartialModel
    {
        //[BindProperty] public Guid Id { get; set; }
        //[BindProperty] public string Name { get; set; }

        [BindProperty] public Guid UserId { get; set; }
        [BindProperty] public string UserEmail { get; set; }

        public void OnGet()
        {
        }

    }
}
