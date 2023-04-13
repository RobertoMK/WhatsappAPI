using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using System;
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
            var parameters = "";
            using var browserFetcher = new BrowserFetcher();
            var chromePath = browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Result;

            LaunchOptions launchOptions = new LaunchOptions { Headless = true, UserDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".local-chromium", "Win64-884014", "chrome-win", "UserData").Replace(@"\", @"\\"), ExecutablePath = chromePath.ExecutablePath };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            Thread.Sleep(1000);
            parameters = string.Concat(url, m.number, "&text=", Uri.EscapeDataString(m.message));

            //puppeteer script start --------------------------------------------
            var page = await browser.NewPageAsync();

            try
            {
                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3641.0 Safari/537.36");
                await page.GoToAsync(parameters);
                var sendMessage = page.WaitForSelectorAsync("[data-testid=\"conversation-compose-box-input\"]").Result;
                Thread.Sleep(1000);
                await page.ClickAsync("[data-testid=\"conversation-compose-box-input\"]");
                Thread.Sleep(100);
                await page.Keyboard.PressAsync("Enter");
                Thread.Sleep(1000);
                await browser.CloseAsync();
            } catch (Exception ex)
            {
                var ts = new DateTime().ToString();
                await page.ScreenshotAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorScreen_" + ts + ".jpg").Replace(@"\", @"\\"));
                _logger.LogError(ex.ToString());
            }

            //puppeteer script end --------------------------------------------
        }
    }
}