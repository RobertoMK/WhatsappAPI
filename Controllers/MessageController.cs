using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;
using System;
using WhatsappAPI.Models;

namespace WhatsappAPI.Controllers
{
    [ApiController]
    [Route("api/whatsapp")]
    public class MessageController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IAuthController _authController;
        private string url;

        public MessageController(ILogger<MessageController> logger)
        {
            _logger = logger;
            url = "https://web.whatsapp.com/send?phone=55";
        }

        [HttpPost]
        public void Send(Message m)
        {
            m.number.ForEach(async x => { await SendMessage(x,m.message); });

        }

        private async Task SendMessage(string number, string message)
        {
            var parameters = "";
            using var browserFetcher = new BrowserFetcher();
            var chromePath = browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Result;

            LaunchOptions launchOptions = new LaunchOptions { Headless = false, UserDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".local-chromium", "Win64-884014", "chrome-win", "UserData").Replace(@"\", @"\\"), ExecutablePath = chromePath.ExecutablePath, Args = new string[] { "--remote-debugging-port=9222" }  };
            ConnectOptions connectOptions = new ConnectOptions { BrowserURL = "http://127.0.0.1:9222" };
            Browser browser;

            try
            {
                browser = (Browser)Puppeteer.LaunchAsync(launchOptions).Result;
            } catch (Exception ex)
            {
                browser = (Browser)Puppeteer.ConnectAsync(connectOptions).Result;
            }
            
            
            parameters = string.Concat(url, number, "&text=", Uri.EscapeDataString(message));

            AuthController authController = new AuthController();
            //puppeteer script start --------------------------------------------
            var page = await browser.NewPageAsync();

            try
            {
                //this user agent prevents whatsapp from detecting puppeteer within chromium
                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3641.0 Safari/537.36");
                await page.GoToAsync(parameters);

                try
                {
                    var sendMessage = page.WaitForSelectorAsync("[data-testid=\"conversation-compose-box-input\"]").Result;
                }
                catch (Exception e)
                {
                    await page.WaitForSelectorAsync("[data-testid=\"qrcode\"]");
                    var qr = authController.GetQrCode((Page)page).Result;
                    Console.WriteLine(qr);
                    var sendMessage = page.WaitForSelectorAsync("[data-testid=\"conversation-compose-box-input\"]").Result;
                }
                finally
                {
                    Thread.Sleep(1000);
                }

                //clicks on the message input box by the data-testid reference
                await page.ClickAsync("[data-testid=\"conversation-compose-box-input\"]");
                Thread.Sleep(100);

                await page.Keyboard.PressAsync("Enter");
                Thread.Sleep(500);

                await browser.CloseAsync();
                //puppeteer script end --------------------------------------------
            }
            catch (Exception ex)
            {
                //await page.ScreenshotAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorScreen.jpg").Replace(@"\", @"\\"));
                _logger.LogError(ex.ToString());
            }
        }
    }
}