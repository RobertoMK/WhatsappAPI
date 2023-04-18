using PuppeteerSharp;

namespace WhatsappAPI.Controllers
{
    public interface IAuthController
    {
        Task<string> GetQrCode(Page page);
        Task<bool> StartProfile();
    }
}
