using PuppeteerSharp;
using QRCoder;
using System.Xml.Linq;
using WhatsappAPI.Exceptions;
using static QRCoder.PayloadGenerator;

namespace WhatsappAPI.Controllers
{
    public class AuthController : IAuthController
    {
        private readonly IBrowserController _browserController;
        public async Task<string> GetQrCode(Page page)
        {
            try
            {
                var qr = await page.WaitForSelectorAsync("[data-testid=\"qrcode\"]");
                //gets the qrcode string from the div on the data-ref attribute
                var value = await qr.EvaluateFunctionAsync<string>("el => el.getAttribute(\"data-ref\")");

                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(value, QRCodeGenerator.ECCLevel.Q);
                AsciiQRCode qrCode = new AsciiQRCode(qrCodeData);
                string qrCodeAsAsciiArt = qrCode.GetGraphic(1);

                return qrCodeAsAsciiArt;
            }
            catch (Exception ex)
            {
                Exception e = new QrCodeNotFoundException("Whatsapp QR Code not found on this instance");
                throw e;
            }

        }

        public async Task<bool> StartProfile()
        {
            /*using var browserFetcher = new BrowserFetcher();
            var chromePath = browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Result;

            LaunchOptions launchOptions = new LaunchOptions { Headless = false, UserDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".local-chromium", "Win64-884014", "chrome-win", "UserData").Replace(@"\", @"\\"), ExecutablePath = chromePath.ExecutablePath };

            await using var browser = await Puppeteer.LaunchAsync(launchOptions);*/

            await using var browser = await _browserController.GetBrowser();
            Thread.Sleep(1000);

            var page = await browser.NewPageAsync();

            await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3641.0 Safari/537.36");
            await page.GoToAsync("https://web.whatsapp.com/");
            try
            {
                var qr = GetQrCode((Page)page).Result;
                Console.WriteLine(qr);
                await page.WaitForSelectorAsync("[data-testid=\"chat-list-search\"]", new WaitForSelectorOptions { Timeout = 40000 });
                await browser.CloseAsync();
                return true;
            }
            catch (QrCodeNotFoundException e1)
            {
                Console.WriteLine(e1.Message);
                Console.WriteLine("Trying with a new QRCode...");
                await browser.CloseAsync();
                return false;
            }
            catch (Exception e2)
            {
                try
                {
                    await page.WaitForSelectorAsync("[data-testid=\"chat-list-search\"]");
                    Console.WriteLine("Whatsapp Session Restored");
                    await page.CloseAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unknown error while opening Whatsapp");
                    await page.CloseAsync();
                    return false;
                }

            }

        }



    }
}
