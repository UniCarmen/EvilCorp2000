using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace EvilCorp2000.Pages.UserManagement
{
    [Authorize(Roles = "CEOofDoom")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<ManageUsersModel> _logger;

        //nur für Tests
        //protected List<IdentityUser> GetUsers() => Users;

        [BindProperty] public string NewUserEmail { get; set; } = string.Empty;
        protected List<IdentityUser> Users { get; set; } = new();
        public List<UserWithRole> UsersWithRoles { get; set; } = new();
        [BindProperty] public string SelectedRole { get; set; } = "TaskDrone";

        public bool ShowDeletionConfirmation { get; set; }// = false;
        [BindProperty] public string UserEmail { get; set; }
        [BindProperty] public Guid UserId { get; set; }


        public ManageUsersModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<ManageUsersModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                Users = await _userManager.Users.ToListAsync();

                foreach (var user in Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    UsersWithRoles.Add(new UserWithRole
                    {
                        Email = user.Email,
                        Role = roles.FirstOrDefault()
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("There was a problem loading the users.", ex);
            }
        }

        public async Task<IActionResult> OnGet()
        {
            ShowDeletionConfirmation = false;
            await LoadDataAsync();
            return Page();
        }


        public async Task<IActionResult> OnPostDeleteUser()
        {
            try
            {
                if(UserEmail == null)
                {
                    ModelState.AddModelError(nameof(UserEmail), "Invalid user.");
                    return await OnGet();
                }

                var existingUser = await _userManager.FindByEmailAsync(UserEmail);
                if (existingUser == null)
                {
                    ModelState.AddModelError(nameof(UserEmail), "User with this email does not exist.");
                    return await OnGet();
                }
                var roles = await _userManager.GetRolesAsync(existingUser);

                await _userManager.DeleteAsync(existingUser);
                //await _userManager.RemoveFromRolesAsync(existingUser, roles);
                return await OnGet();
            }
            catch (Exception ex)
            {
                _logger.LogError("There was a problem deleting the user.", ex);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostNewUser()
        {
            try
            {
                //ModelState.Clear();

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

                // TaskDrone created: drone123@evilcorp2000.com | Password: HDTr+6 
                //drone795@evilcorp2000.com | Password: 5-2MdC
                //Overseer created: overseerBiggs@evilcorp2000.com | Password: D2*c6-
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


        public async Task<IActionResult> OnPostShowDeletionInformation()//(string userEmail)
        {
            //ModelState.Clear();
            await LoadDataAsync();
            ShowDeletionConfirmation = true;

            var existingUser = await _userManager.FindByEmailAsync(UserEmail);
            UserId = Guid.Parse(existingUser.Id);

            return Page();
        }

        public async Task<IActionResult> OnPostHideDeletionInformation()//(string userEmail)
        {
            //ModelState.Clear();
            await LoadDataAsync();
            //ShowDeletionConfirmation = false; --> nicht nötig, weil standardmäßig auf false
            return Page();
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


        public class UserWithRole
        {
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
