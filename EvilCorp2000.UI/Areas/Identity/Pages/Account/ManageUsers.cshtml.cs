using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EvilCorp2000.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "CEOofDoom")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ManageUsersModel> _logger;

        [BindProperty] public string NewUserEmail { get; set; } = string.Empty;
        private List<IdentityUser> Users { get; set; } = new();
        public List<UserWithRole> UsersWithRoles { get; set; } = new();
        [BindProperty] public string SelectedRole { get; set; } = "TaskDrone";


        public ManageUsersModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ManageUsersModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        

        public async Task<IActionResult> OnGet()
        {      
            try
            {
                Users = _userManager.Users.ToList();

                foreach (var user in Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    UsersWithRoles.Add(new UserWithRole
                    {
                        Email = user.Email,
                        Role = roles.FirstOrDefault()
                    });
                }

                return Page();
            }
            catch (Exception ex)
            {
                _logger.LogError("There was a problem getting the users.", ex);
                return Page();
            }
        }

        public async Task<IActionResult> OnPost()
        {
            try
            {
                ModelState.Clear();

                if (string.IsNullOrWhiteSpace(NewUserEmail) || !new EmailAddressAttribute().IsValid(NewUserEmail))
                {
                    ModelState.AddModelError(nameof(NewUserEmail), "Invalid email address.");
                    return await OnGet();
                }


                var existingUser = await _userManager.FindByEmailAsync(NewUserEmail);
                if (existingUser != null)
                {
                    ModelState.AddModelError(nameof(NewUserEmail), "A user with this email already exists.");
                    return await OnGet();
                }

                // Sicherstellen, dass die Rolle gültig ist
                if (SelectedRole != "TaskDrone" && SelectedRole != "Overseer")
                {
                    ModelState.AddModelError(nameof(NewUserEmail), "Invalid role selected.");
                    return await OnGet();
                }

                var newUser = new IdentityUser { UserName = NewUserEmail, Email = NewUserEmail };
                string password = GenerateSecurePassword();

                var result = await _userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, SelectedRole);
                    Console.WriteLine($"This simulates an E-Mail: {SelectedRole} created: {NewUserEmail} | Password: {password}. Please Log in and change your password.");

                    NewUserEmail = string.Empty;
                    SelectedRole = "TaskDrone";

                    return await OnGet();
                }

                // TaskDrone created: drone123@evilcorp2000.com | Password: HDTr+6. 
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }


                return await OnGet();
            }
            catch (Exception ex)
            {
                _logger.LogError("There was a problem saving the new user.", ex);
                return Page();
            }
        }





        public class UserWithRole
        {
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }


        public string GenerateSecurePassword()
        {
            var passwordOptions = _userManager.Options.Password;
            int length = passwordOptions.RequiredLength;

            string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            string digitChars = "0123456789";
            string specialChars = "!@#$%^&*()-_=+";

            var rnd = new Random();
            var passwordChars = new List<char>();

            // Garantiert, dass jede aktivierte Kategorie mindestens ein Zeichen enthält
            if (passwordOptions.RequireUppercase) passwordChars.Add(upperCaseChars[rnd.Next(upperCaseChars.Length)]);
            if (passwordOptions.RequireLowercase) passwordChars.Add(lowerCaseChars[rnd.Next(lowerCaseChars.Length)]);
            if (passwordOptions.RequireDigit) passwordChars.Add(digitChars[rnd.Next(digitChars.Length)]);
            if (passwordOptions.RequireNonAlphanumeric) passwordChars.Add(specialChars[rnd.Next(specialChars.Length)]);

            // Auffüllen mit zufälligen Zeichen bis zur geforderten Länge
            string allValidChars = upperCaseChars + lowerCaseChars + digitChars + specialChars;
            while (passwordChars.Count < length)
            {
                passwordChars.Add(allValidChars[rnd.Next(allValidChars.Length)]);
            }

            // Passwort durchmischen und als String zurückgeben
            return new string(passwordChars.OrderBy(_ => rnd.Next()).ToArray());
        }
    }
}
