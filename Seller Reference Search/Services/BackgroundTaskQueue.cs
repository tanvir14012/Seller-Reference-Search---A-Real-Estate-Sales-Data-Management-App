using System.Threading.Channels;

namespace Seller_Reference_Search.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<Func<IServiceProvider, CancellationToken, Task>> _queue;
        private readonly ILogger<BackgroundTaskQueue> _logger;
        private readonly IServiceProvider _serviceProvider;

        public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger, IServiceProvider serviceProvider, int capacity = 100)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                SingleReader = true,
                SingleWriter = false,
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<Func<IServiceProvider, CancellationToken, Task>>(options);
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task QueueBackgroundWorkItemAsync(Func<IServiceProvider, CancellationToken, Task> workItem)
        {
            if (workItem == null)
                throw new ArgumentNullException(nameof(workItem));

            try
            {
                await _queue.Writer.WriteAsync(workItem);
                _logger.LogInformation("Successfully queued a background work item.");
            }
            catch (ChannelClosedException)
            {
                _logger.LogError("Failed to queue background work item as the channel is closed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while queueing a background work item");
                throw;
            }
        }

        public async Task<Func<IServiceProvider, CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            try
            {
                var workItem = await _queue.Reader.ReadAsync(cancellationToken);
                _logger.LogInformation("Successfully dequeued a background work item.");
                return workItem;
            }
            catch (ChannelClosedException)
            {
                _logger.LogError("Failed to dequeue background work item as the channel is closed.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while dequeuing a background work item.");
                throw;
            }
        }
    }
}