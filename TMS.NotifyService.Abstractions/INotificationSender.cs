namespace TMS.NotifyService.Abstractions
{
    public interface INotificationSender
    {
        /// <summary>
        /// Отправить уведомление получателю.
        /// </summary>
        /// <param name="target">Адрес получателя (email, телефон, chatId и т.д.)</param>
        /// <param name="message">Текст сообщения</param>
        /// <returns>Task</returns>
        Task SendAsync(string target, string message);
    }
}