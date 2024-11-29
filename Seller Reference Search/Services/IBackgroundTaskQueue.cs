namespace Seller_Reference_Search.Services
{
    public interface IBackgroundTaskQueue
    {
        Task QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, Task> workItem);
        Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}
