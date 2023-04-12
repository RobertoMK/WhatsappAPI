using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using WhatsappAPI.Models;

namespace WhatsappAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private string url;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
            url = "https://web.whatsapp.com/send?phone=55";
        }

        [HttpPost]
        public async void Send(Message m)
        {
            //string[] args = new string ["--disable-gpu", "--disable-setuid-sandbox", "--no-sandbox", "--no-zygote"];

            var parameters = "";
            using var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            var chromePath = browserFetcher.GetExecutablePath(BrowserFetcher.DefaultChromiumRevision);

            LaunchOptions launchOptions = new LaunchOptions { Headless = true, UserDataDir = "./tmp", Args = new string[] { "--disable-setuid-sandbox", "--no-sandbox", "--no-zygote" } };

            var browser = Puppeteer.LaunchAsync(launchOptions).Result;
            Thread.Sleep(1000);
            parameters = string.Concat(url, m.number, "&text=", Uri.EscapeDataString(m.message));

            //puppeteer script start --------------------------------------------
            var page = await browser.NewPageAsync();
            await page.GoToAsync(parameters);

            var sendMessage = page.WaitForSelectorAsync("[data-testid=\"conversation-compose-box-input\"]").Result;
            Thread.Sleep(1000);
            await page.ClickAsync("[data-testid=\"conversation-compose-box-input\"]");
            Thread.Sleep(100);
            await page.Keyboard.PressAsync("Enter");
            Thread.Sleep(100);
            await browser.CloseAsync();
            //puppeteer script end --------------------------------------------
        }
    }
}