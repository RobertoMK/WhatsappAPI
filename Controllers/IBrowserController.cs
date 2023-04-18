using PuppeteerSharp;

namespace WhatsappAPI.Controllers
{
    public interface IBrowserController
    {
        Task<IBrowser> GetBrowser();
    }
}
