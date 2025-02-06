using DataAccess.DBContexts;
using DataAccess.Repositories;
using BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using EvilCorp2000.Areas.Identity.Data;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//f�r ASPNETCORE Identity
builder.Services.AddDbContext<AuthContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Kein E-Mail-Confirm n�tig
})
.AddRoles<IdentityRole>() // Rollenverwaltung aktivieren
.AddEntityFrameworkStores<AuthContext>() //alle Informationen in DB speichern �ber EF Core
.AddDefaultTokenProviders() //tokenbasierte Features: PasswortR�cksetzung, Emailbest�t. (falls aktiviert), 2FA
.AddDefaultUI(); //nutzt vorgefertige Login/out/Register seiten

builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.RoleClaimType = "role"; // Stellt sicher, dass die Rollen geladen / verwendet werden
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanDeleteProducts", policy =>
        policy.RequireRole("Overseer", "CEOofDoom"));
});


// Serilog-Konfiguration aus der appsettings.json laden
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddDbContext<EvilCorp2000Context>(options =>
    options.UseSqlServer(connectionString));

//erzwingt Https und leitet automatisch zu https, selbst wenn explizit http angegeben wurde.
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IProductForSaleManager, ProductForSaleManager>();
builder.Services.AddScoped<IInternalProductManager, InternalProductManager>();

////Global AutoValidateAntiForgeryTokens for Submits
//builder.Services.AddControllersWithViews(options =>
//{
//    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
//});

//Serilog hinzuf�gen

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.AuthorizeFolder("/ProductManagement"); // Sperrt das Produktmanagement f�r nicht eingeloggte Benutzer
    });

var app = builder.Build();



app.UseSerilogRequestLogging();

if (!app.Environment.IsDevelopment())
{
    //Error Handling - unerwartete Fehler
    app.UseExceptionHandler("/Pages/Error");
    //Weiterleitung bei erwarteten Fehlern
    app.UseStatusCodePagesWithReExecute("/Pages/Error/{0}"); 
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//F�r die Produktion muss auch ein SSL Zertifikat f�r HTTPS Seiten erstellt werden, z. B. �ber  Let's Encrypt
//In Dev wird von VS automatisch eins eingerichtet
app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    // X-Content-Type-Options Header setzen
    //context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

    //// X-Frame-Options Header setzen
    //context.Response.Headers.Append("X-Frame-Options", "DENY");

    // Content Security Policy setzen
    //context.Response.Headers.Append("Content-Security-Policy", "default-src 'self'; script-src 'self'; style-src 'self'");    zu streng, Bootstrap funktioniert nicht richtig
    //context.Response.Headers.Append("Content-Security-Policy",
    //"default-src 'self'; " +
    //"script-src 'self' https://cdn.jsdelivr.net; " +
    //"style-src 'self' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
    //"font-src 'self' https://cdnjs.cloudflare.com; " +
    //"connect-src 'self' http://localhost:* ws://localhost:* wss://localhost:*; " +
    //"img-src 'self' data:;");

    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();


//Vor�bergehend: Hardcoded Benutzer, da noch keine Registrierung vorhanden ist
//beim ersten Start, werden die Rollen automatisch erstellt

//Erstellt einen untergeordneten Scope f�r gesamte Methode CreateRoles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await CreateRolesAndAdmin(services);
}


app.Run();



//TODO: wenn man das nicht hardcoded, was macht man denn damit???
async Task CreateRolesAndAdmin(IServiceProvider serviceProvider)
{
    {
        //services aus dem DI Container abrufen -> IServiceProvider
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>(); //verwaltet Rollen
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); //Benutzer anlegen, Rollen zuweisen

        //Erstellen der Rollen
        string[] roles = { "CEOofDoom", "TaskDrone", "Overseer" };


        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Admin anlegen
        var adminEmail = "ceo@evilcorp.com";
        //var adminName = "EvilCeo"; //nur buchstaben oder zahlen
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
            await userManager.CreateAsync(adminUser, "EvilPassword123!");
            await userManager.AddToRoleAsync(adminUser, "CEOofDoom");
        }
    }
}

await CreateRolesAndAdmin(app.Services);

//ceo@evilcorp.com
//EvilPassword123!