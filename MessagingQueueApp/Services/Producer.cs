using MessagingQueueApp.Models;
using ILogger = Serilog.ILogger; 
namespace MessagingQueueApp.Services
{
    public class Producer
    {
        private readonly IQueueProvider _queue;
        private readonly MessageRepository _repo;
        private readonly ILogger _logger;

        public Producer(IQueueProvider queue, MessageRepository repo, ILogger logger)
        {
            _queue = queue;
            _repo = repo;
            _logger = logger;
        }

        public async Task<Guid> ProduceAsync(Message message)
        {
            message.Id = Guid.NewGuid();
            message.Status = MessageStatus.Pending;
            message.CreatedAt = DateTime.UtcNow;
            message.RetryCount = 0;
            message.Priority = message.Priority == 0 ? 1 : message.Priority;

            await _repo.AddAsync(message);
            _queue.Enqueue(message);

            _logger.Information("Produced message {Id} of type {Type}", message.Id, message.Type);
            return message.Id;
        }
    }
}
