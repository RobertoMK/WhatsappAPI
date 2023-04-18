using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using WhatsappAPI.Controllers;

namespace WhatsappAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class BrowserInitialization
    {
        private readonly RequestDelegate _next;
        private readonly IBrowserController _browserController;

        public BrowserInitialization(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class BrowserInitializationExtensions
    {
        public static IApplicationBuilder UseBrowserInitialization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<BrowserInitialization>();
        }
    }
}
