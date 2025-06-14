namespace TMS.NotifyService.Abstractions
{
    public interface IEventConsumer<TEvent>
    {
        Task ConsumeAsync(TEvent evt);
    }
}
