using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using TMS.NotifyService.Abstractions;

namespace TMS.NotifyService.Telegram
{
    public class TelegramNotifier : INotificationSender
    {
        private readonly string _botToken;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<TelegramNotifier> _logger;

        public TelegramNotifier(string botToken, IHttpClientFactory clientFactory, ILogger<TelegramNotifier> logger)
        {
            _botToken = botToken ?? throw new ArgumentNullException(nameof(botToken));
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(string chatId, string message = null)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
                var data = new Dictionary<string, string>
                {
                    { "chat_id", chatId },
                    { "text", message }
                };
                var content = new FormUrlEncodedContent(data);
                var response = await client.PostAsync(url, content);

                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Telegram API Error: {response.StatusCode}, Response: {responseString}");
                }
                else
                {
                    _logger.LogInformation($"Telegram API Response: {responseString}");
                }

                try
                {
                    JObject jsonResponse = JObject.Parse(responseString);
                    if (jsonResponse["ok"].Value<bool>() == false)
                    {
                        _logger.LogError($"Telegram API returned error: {jsonResponse["description"]}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error parsing Telegram API response");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to Telegram");
            }
        }
    }
}