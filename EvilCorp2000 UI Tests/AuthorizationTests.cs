using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace EvilCorp2000_UI_Tests
{
    public class AuthorizationTests
    {
        private readonly IAuthorizationService _authorizationService;

        //ServiceCollection aufbauen, fast wie in der Program.cs - Mock für das echte ASP.NET Core DI-System
        //nutzung von IAuthService ohne Starten der App
        public AuthorizationTests()
        {
            var services = new ServiceCollection();
            //notwendig für Tests mit IAuthService
            services.AddLogging();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanDeleteProducts", policy =>
                    policy.RequireRole("Overseer", "CEOofDoom"));
            });

            //IAuthorizationServie verfügbar machen
            var provider = services.BuildServiceProvider();
            _authorizationService = provider.GetRequiredService<IAuthorizationService>();
        }

        [Theory]
        [InlineData("Overseer", true)]
        [InlineData("CEOofDoom", true)]
        [InlineData("TaskDrone", false)]
        [InlineData("Customer", false)]
        public async Task CanDeleteProductsPolicy_ShouldAllowCorrectRoles(string role, bool shouldSucceed)
        {
            // Arrange: Benutzer mit bestimmter Rolle erstellen
            var user = new ClaimsPrincipal(new ClaimsIdentity(
                    new[]
                    {
                        new Claim(ClaimTypes.Role, role)
                    }, "TestAuth" // Platzhalter für das Schema
                ));

            // Act: Überprüfen, ob der Benutzer die Policy erfüllt
            var result = await _authorizationService.AuthorizeAsync(user, null, "CanDeleteProducts");

            // Assert: Funktioniert die Autorisierung wie erwartet?
            Assert.Equal(shouldSucceed, result.Succeeded);
        }
    }
}
