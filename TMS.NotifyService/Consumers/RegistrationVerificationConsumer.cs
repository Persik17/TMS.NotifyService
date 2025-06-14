using TMS.NotifyService.Abstractions;
using TMS.Contracts.Events;
using TMS.Contracts.Enums;
using Microsoft.Extensions.DependencyInjection;
using TMS.NotifyService.Email;
using TMS.NotifyService.Phone;
using TMS.NotifyService.Telegram;
using System;
using System.Threading.Tasks;
using System.Linq;
using MassTransit;

namespace TMS.NotifyService.Consumers
{
    public class RegistrationVerificationConsumer : IConsumer<RegistrationVerificationCreatedEvent>
    {
        private readonly IServiceProvider _provider;

        public RegistrationVerificationConsumer(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public async Task Consume(ConsumeContext<RegistrationVerificationCreatedEvent> context)
        {
            var evt = context.Message;

            INotificationSender? sender = evt.Type switch
            {
                (int)VerificationType.Email => _provider.GetServices<INotificationSender>().OfType<EmailNotifier>().FirstOrDefault(),
                (int)VerificationType.Phone => _provider.GetServices<INotificationSender>().OfType<SmsNotifier>().FirstOrDefault(),
                (int)VerificationType.Telegram => _provider.GetServices<INotificationSender>().OfType<TelegramNotifier>().FirstOrDefault(),
                _ => null
            };

            if (sender != null)
            {
                var message = $"Ваш код подтверждения: {evt.Code}";
                await sender.SendAsync(evt.Target, message);
            }
            else
            {
                Console.WriteLine($"No INotificationSender found for VerificationType: {evt.Type}");
            }
        }
    }
}