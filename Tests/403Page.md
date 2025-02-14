### **Benutzerdefinierte 403-Fehlermeldung in ASP.NET Core (Razor Pages)**  

Wenn ein Benutzer **nicht autorisiert ist**, wird standardmäßig eine **leere 403 Forbidden-Seite** zurückgegeben. Du kannst eine eigene **403-Seite mit einer besseren Benutzerführung erstellen**.

---

## **🟢 1. `StatusCodePages` aktivieren und eigene 403-Seite definieren**  
In **`Program.cs`** musst du sicherstellen, dass `StatusCodePages` aktiviert ist und eine benutzerdefinierte **Fehlerseite für 403** aufgerufen wird.

Füge dies in die Middleware-Kette ein:

```csharp
app.UseStatusCodePagesWithReExecute("/Error/{0}");
```

Dadurch wird jeder **403-Fehler** auf eine benutzerdefinierte Seite umgeleitet.

---

## **🟢 2. Erstelle eine `Error`-Seite für Fehlercodes**
Erstelle eine **Razor Page** unter:  
📂 **`Pages/Error.cshtml`**  

### **📌 `Error.cshtml` (die Fehlerseite)**
```razor
@page "{code:int?}"
@model ErrorModel
@{
    ViewData["Title"] = "Error";
}

<h1 class="text-danger">Error @Model.StatusCode</h1>

@if (Model.StatusCode == 403)
{
    <p>Access Denied. You do not have permission to view this page.</p>
    <a href="/" class="btn btn-primary">Go to Home</a>
}
else
{
    <p>An unexpected error occurred.</p>
}
```

---

### **📌 `Error.cshtml.cs` (Code-Behind der Fehlerseite)**
```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ErrorModel : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int? StatusCode { get; set; }

    public void OnGet(int? code)
    {
        StatusCode = code ?? 500;
    }
}
```

---

## **🟢 3. Falls du `Forbid()` verwendest, leite zur Fehlerseite weiter**
Falls du `return Forbid();` nutzt, kannst du es so anpassen:

```csharp
return RedirectToPage("/Error", new { code = 403 });
```

---

## **🔴 Fazit – Was passiert jetzt?**
✅ **Alle 403-Fehler werden auf `/Error/403` umgeleitet**  
✅ **Andere Fehler (z. B. 404, 500) können ebenfalls hier verarbeitet werden**  
✅ **Die Seite zeigt eine Benutzerfreundliche Meldung statt einer leeren 403-Seite**  

Falls du noch Anpassungen brauchst, sag Bescheid! 🚀😊
