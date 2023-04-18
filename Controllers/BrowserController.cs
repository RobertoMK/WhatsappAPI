using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace WhatsappAPI.Controllers
{
    public class BrowserController : IBrowserController
    {


        public async Task<IBrowser> GetBrowser()
        {      
            using var browserFetcher = new BrowserFetcher();
            var chromePath = browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision).Result;
            LaunchOptions launchOptions = new LaunchOptions { Headless = false, UserDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".local-chromium", "Win64-884014", "chrome-win", "UserData").Replace(@"\", @"\\"), ExecutablePath = chromePath.ExecutablePath };
            await using var browser = await Puppeteer.LaunchAsync(launchOptions);
            
            return browser;
        }
    }
}
