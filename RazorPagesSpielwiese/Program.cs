using DataAccess.DBContexts;
using DataAccess.Repositories;
using BusinessLayer.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Serilog-Konfiguration aus der appsettings.json laden
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});


builder.Services.AddDbContext<EvilCorp2000Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

//Serilog hinzufügen

builder.Services.AddRazorPages();

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

//Für die Produktion muss auch ein SSL Zertifikat für HTTPS Seiten erstellt werden, z. B. über  Let's Encrypt
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

app.UseAuthorization();

app.MapRazorPages();

app.Run();
