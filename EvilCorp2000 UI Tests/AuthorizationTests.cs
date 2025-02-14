using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace EvilCorp2000_UI_Tests
{
    public class AuthorizationTests
    {
        private readonly IAuthorizationService _authorizationService;

        //ServiceCollection aufbauen, fast wie in der Program.cs - Mock f�r das echte ASP.NET Core DI-System
        //nutzung von IAuthService ohne Starten der App
        public AuthorizationTests()
        {
            var services = new ServiceCollection();
            //notwendig f�r Tests mit IAuthService
            services.AddLogging();
            services.AddAuthorizationCore(options =>
            {
                options.AddPolicy("CanDeleteProducts", policy =>
                    policy.RequireRole("Overseer", "CEOofDoom"));
            });

            //IAuthorizationServie verf�gbar machen
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
                    }, "TestAuth" // Platzhalter f�r das Schema
                ));

            // Act: �berpr�fen, ob der Benutzer die Policy erf�llt
            var result = await _authorizationService.AuthorizeAsync(user, null, "CanDeleteProducts");

            // Assert: Funktioniert die Autorisierung wie erwartet?
            Assert.Equal(shouldSucceed, result.Succeeded);
        }
    }
}
