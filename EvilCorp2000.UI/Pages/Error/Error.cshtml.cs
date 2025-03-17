

//LÖSCHEN!!

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace EvilCorp2000.Pages.Error
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            var path = HttpContext.Request.Path;
            var statusCode = HttpContext.Response.StatusCode;
            var method = HttpContext.Request.Method;
            var queryString = HttpContext.Request.QueryString.HasValue ? HttpContext.Request.QueryString.Value : "none";
            var user = User?.Identity?.Name ?? "Anonymous";
            var routeData = RouteData.Values;

            _logger.LogError("Error page triggered. " +
                     "RequestId: {RequestId}, " +
                     "Path: {Path}, " +
                     "StatusCode: {StatusCode}, " +
                     "Method: {Method}, " +
                     "Query: {QueryString}, " +
                     "User: {User}, " +
                     "RouteData: {@RouteData}",
                     RequestId, path, statusCode, method, queryString, user, routeData);
        }
    }

}
