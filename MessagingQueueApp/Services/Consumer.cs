using MessagingQueueApp.Models;
using ILogger = Serilog.ILogger;

namespace MessagingQueueApp.Services
{
    public class Consumer : BackgroundService
    {
        private readonly IQueueProvider _queue;
        private readonly MessageRepository _repo;
        private readonly ILogger _logger;

        public Consumer(IQueueProvider queue, MessageRepository repo, ILogger logger)
        {
            _queue = queue;
            _repo = repo;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Information("Message consumer started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                if (_queue.TryDequeue(out var message))
                {
                    try
                    {
                        _logger.Information("Processing message {Id} to {Recipient}", message.Id, message.Recipient);

                        // Simulate delivery logic
                        await Task.Delay(500); // Simulated delay

                        message.Status = MessageStatus.Processed;
                        message.ProcessedAt = DateTime.UtcNow;
                        await _repo.UpdateAsync(message);

                        _logger.Information("Delivered message {Id}", message.Id);
                    }
                    catch (Exception ex)
                    {
                        message.RetryCount++;
                        message.Status = MessageStatus.Failed;
                        _logger.Error(ex, "Failed to deliver message {Id}", message.Id);
                        await _repo.UpdateAsync(message);
                    }
                }
                else
                {
                    await Task.Delay(200); // No messages, wait a bit
                }
            }

            _logger.Information("Message consumer stopped.");
        }
    }
}
