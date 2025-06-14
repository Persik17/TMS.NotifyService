using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using TMS.NotifyService.Abstractions;
using TMS.NotifyService.Consumers;
using TMS.NotifyService.Email;
using TMS.NotifyService.Phone;
using TMS.NotifyService.Telegram;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using TMS.NotifyService.Options;

namespace TMS.NotifyService
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient();

                    services.AddLogging(configure => configure.AddConsole());

                    services.Configure<TelegramOptions>(hostContext.Configuration.GetSection("Telegram"));
                    services.Configure<SmsOptions>(hostContext.Configuration.GetSection("Sms"));
                    services.Configure<EmailOptions>(hostContext.Configuration.GetSection("Email"));

                    services.AddSingleton<INotificationSender, SmsNotifier>(sp =>
                    {
                        var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SmsOptions>>().Value;
                        return new SmsNotifier(options.ApiUrl, options.ApiKey);
                    });

                    services.AddSingleton<INotificationSender, EmailNotifier>(sp =>
                    {
                        var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<EmailOptions>>().Value;
                        return new EmailNotifier(options.SmtpHost, options.SmtpPort, options.SmtpUser, options.SmtpPassword, options.FromEmail);
                    });

                    services.AddSingleton(sp =>
                    {
                        var options = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<TelegramOptions>>().Value;
                        var clientFactory = sp.GetRequiredService<IHttpClientFactory>();
                        var logger = sp.GetRequiredService<ILogger<TelegramNotifier>>();
                        return new TelegramNotifier(options.BotToken, clientFactory, logger);
                    });
                    services.AddSingleton<INotificationSender>(sp => sp.GetRequiredService<TelegramNotifier>());

                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<RegistrationVerificationConsumer>(cfg =>
                        {
                            cfg.UseMessageRetry(retryConfig =>
                            {
                                retryConfig.Interval(3, TimeSpan.FromSeconds(5));
                            });
                        });

                        x.UsingRabbitMq((context, cfg) =>
                        {
                            cfg.Host("rabbitmq://localhost", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            cfg.ReceiveEndpoint("registration_verification", e =>
                            {
                                e.ConfigureConsumer<RegistrationVerificationConsumer>(context);
                            });
                        });
                    });
                })
                .UseConsoleLifetime();

            var app = builder.Build();
            Console.WriteLine("Сервис запущен. Ожидание сообщений...");
            await app.RunAsync();
        }
    }
}