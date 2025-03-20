using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;

namespace EvilCorp2000.Pages.UserManagement
{
    [Authorize(Roles = "CEOofDoom")]
    public class ManageUsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
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
            _logger = logger;
        }

        public async Task LoadDataAsync()
        {
            try
            {
                Users = await _userManager.Users.ToListAsync();

                if (Users.IsNullOrEmpty())
                {
                    HandleWarning($"{Users} is null or empty.", "Users couldn't be loaded");
                    return;
                }

                foreach (var user in Users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    UsersWithRoles.Add(new UserWithRole
                    {
                        Email = user.Email,
                        Role = roles.FirstOrDefault() ?? "No Role"
                    });
                }
            }
            catch (Exception ex)
            {
                ExceptionHandling(ex, 
                    $"There was an error loading {nameof(Users)} and {nameof(UsersWithRoles)}. :StackTrace {ex.StackTrace}", 
                    "Users couldn't be loaded. Please try again");
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
                if (UserEmail == null)
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

                return await OnGet();
            }
            catch (Exception ex)
            {
                return ExceptionHandling(ex,
                    $"There was a problem deleting user {UserEmail}. StackTrace: {ex.StackTrace}", 
                    "There was a problem deleting the user.");
            }
        }

        public async Task<IActionResult> OnPostNewUser()
        {
            try
            {
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
                    ModelState.AddModelError(nameof(SelectedRole), "Invalid role selected.");
                    return await OnGet();
                }

                //INFO: darf erst nach der Validierung kommen = erst, wenn das Formular korrekt gefüllt ist
                //INFO: wenn es zu früh kommt, wird der Post nicht ausgeführt - erst, wenn mal ein zweites Mal klickt.
                ModelState.Clear();


                var newUser = new IdentityUser { UserName = NewUserEmail, Email = NewUserEmail };
                string password = GenerateSecurePassword();

                var result = await _userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newUser, SelectedRole);
                    Console.WriteLine(
                        $"This simulates an E-Mail: {SelectedRole} created: {NewUserEmail} | Password: {password}. Please Log in and change your password.");

                    NewUserEmail = string.Empty;
                    SelectedRole = "TaskDrone";

                    return await OnGet();
                }

                //INFO: TaskDrone created: drone123@evilcorp2000.com | Password: HDTr+6 
                //INFO: drone795@evilcorp2000.com | Password: 5-2MdC
                //INFO: Overseer created: overseerBiggs@evilcorp2000.com | Password: D2*c6-
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
                return ExceptionHandling(
                    ex,
                    $"There was a problem saving the new user {UserEmail} with role {SelectedRole}. StackTrace: {ex.StackTrace}", 
                    "There was a problem saving the user.");
            }
        }


        public async Task<IActionResult> OnPostToggleDeletionInformation(bool showDeletionInformation)
        {
            try
            {
                await LoadDataAsync();

                ModelState.Clear();

                if (showDeletionInformation)
                {
                    ShowDeletionConfirmation = true;

                    var existingUser = await _userManager.FindByEmailAsync(UserEmail);

                    var userId = Guid.Empty;

                    if (existingUser == null || !Guid.TryParse(existingUser.Id, out userId))
                    {
                        UserId = userId;
                        ModelState.AddModelError(nameof(UserEmail), "Invalid user.");
                        return Page();
                    }
                }

                return Page();
            }
            catch (Exception ex)
            {
                return ExceptionHandling(
                    ex, 
                    $"There was a problem getting user {UserEmail}. StackTrace: {ex.StackTrace}", 
                    "There was a problem getting the user.");
            }

        }


        private IActionResult ExceptionHandling(Exception ex, string logMessage, string userMessage)
        {
            _logger.LogError(ex, logMessage);
            ModelState.AddModelError(string.Empty, userMessage);
            return Page();
        }


        private void HandleWarning(string logMessage, string userMessage)
        {
            _logger.LogError(logMessage);
            ModelState.AddModelError(string.Empty, userMessage);
        }


        public string GenerateSecurePassword()
        {
            var passwordOptions = _userManager.Options.Password;
            int length = passwordOptions.RequiredLength;

            string upperCaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerCaseChars = "abcdefghijklmnopqrstuvwxyz";
            string digitChars = "0123456789";
            string specialChars = "!@#$%^&*()-_=+";

            var rng = RandomNumberGenerator.Create(); //Random(); --> kryptographisch nicht sicher
            var passwordChars = new List<char>();

            // Garantiert, dass jede aktive Kategorie mindestens ein Zeichen enthält
            if (passwordOptions.RequireUppercase) passwordChars.Add(GetRandomChar(upperCaseChars, rng));
            if (passwordOptions.RequireLowercase) passwordChars.Add(GetRandomChar(lowerCaseChars, rng));
            if (passwordOptions.RequireDigit) passwordChars.Add(GetRandomChar(digitChars, rng));
            if (passwordOptions.RequireNonAlphanumeric) passwordChars.Add(GetRandomChar(specialChars, rng));

            // Auffüllen mit zufälligen Zeichen bis zur geforderten Länge
            string allValidChars = upperCaseChars + lowerCaseChars + digitChars + specialChars;
            while (passwordChars.Count < length)
            {
                passwordChars.Add(GetRandomChar(allValidChars, rng));
            }

            // Passwort durchmischen und als String zurückgeben
            return new string(passwordChars.OrderBy(_ => GetRandomNumber(rng)).ToArray());
        }


        private static char GetRandomChar(string chars, RandomNumberGenerator rng)
        {
            byte[] randomByte = new byte[1];
            rng.GetBytes(randomByte);
            return chars[randomByte[0] % chars.Length]; // Sicherstellen, dass wir in den Index-Bereich kommen
        }

        private static int GetRandomNumber(RandomNumberGenerator rng)
        {
            byte[] randomBytes = new byte[4]; // 32-Bit Integer
            rng.GetBytes(randomBytes);
            return BitConverter.ToInt32(randomBytes, 0) & int.MaxValue; // Positiven Wert erzeugen
        }

        public class UserWithRole
        {
            public string Email { get; set; } = string.Empty;
            public string Role { get; set; } = string.Empty;
        }
    }
}
