using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;
using WhatsappAPI.Models;

namespace WhatsappAPI.Controllers
{
    [Route("api/sms")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly ILogger<MessageController> _logger;
        private readonly IConfiguration _configuration;
        private static readonly HttpClient client = new HttpClient();
        

        public SmsController(ILogger<MessageController> logger, IConfiguration config)
        {
            _logger = logger;
            _configuration = config;
        }

        [HttpGet]
        public void SendSms(Message m)
        {
            try
            {
                m.number.ForEach(x => { sendSmsMessage(x, m.message); });
            } catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            

        }

        private void sendSmsMessage(string number, string message)
        {
            //TODO: include sms api call
            var m = new Regex(@"(?:\*)(?:(?!\s))((?:(?!\*).)+)(?:\*)").Replace(message, "$1");
            m = new Regex(@"(?:_)(?:(?!\s))((?:(?!_).)+)(?:_)").Replace(m, "$1");
            m = new Regex(@"(?:~)(?:(?!\s))((?:(?!~).)+)(?:~)").Replace(m, "$1");
            m = new Regex(@"(?:__)(?:(?!\s))((?:(?!__).)+)(?:__)").Replace(m, "$1");

            var url = _configuration.GetValue<string>("SmsUrl");
            var apiKey = _configuration.GetValue<string>("SmsKey:token");
            var apiLogin = _configuration.GetValue<string>("SmsKey:login");
            url = string.Concat(url, "?acao=sendsms&login=", "", "&token=", "", "&numero=", number, "&msg=", Uri.EscapeDataString(m));
            
            var response = client.GetAsync(url).Result;
            
            if(response != null) {
                var contentStream = response.Content.ReadAsStreamAsync().Result;

                try
                {
                    var json = System.Text.Json.JsonSerializer.DeserializeAsync<Status>(contentStream, new System.Text.Json.JsonSerializerOptions { IgnoreNullValues = true, PropertyNameCaseInsensitive = true }).Result;

                    if (!json.status.Equals("success"))
                    {
                        
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError("Bad response from SMS server");
                }
            }
        }
    }
}
