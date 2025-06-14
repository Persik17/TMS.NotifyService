using TMS.NotifyService.Abstractions;

namespace TMS.NotifyService.Phone
{
    public class SmsNotifier : INotificationSender
    {
        private readonly string _apiUrl;
        private readonly string _apiKey;

        public SmsNotifier(string apiUrl, string apiKey)
        {
            _apiUrl = apiUrl;
            _apiKey = apiKey;
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            using var client = new HttpClient();

            // Пример для абстрактного API, замените на параметры вашего провайдера
            var data = new Dictionary<string, string>
            {
                { "to", phoneNumber },
                { "text", message },
                { "apiKey", _apiKey }
            };

            var content = new FormUrlEncodedContent(data);
            var response = await client.PostAsync(_apiUrl, content);

            response.EnsureSuccessStatusCode();
        }
    }
}