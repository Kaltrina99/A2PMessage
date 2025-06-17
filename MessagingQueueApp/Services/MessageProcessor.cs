using MessagingQueueApp.Models;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MessagingQueueApp.Services;

public class MessageProcessor
{
    private readonly IQueueProvider _queue;
    private readonly MessageRepository _repo;
    private readonly ILogger _logger;
    private readonly int _maxDegreeOfParallelism = 4;
    private readonly int _throttleDelayMs = 250;
    private readonly int _maxRetry = 3;

    public MessageProcessor(IQueueProvider queue, MessageRepository repo, ILogger logger)
    {
        _queue = queue;
        _repo = repo;
        _logger = logger;
    }

    public void Start()
    {
        Task.Run(ProcessLoop);
    }

    private async Task ProcessLoop()
    {
        var tasks = new List<Task>();
        while (true)
        {
            while (_queue.TryDequeue(out var msg))
            {
                tasks.Add(ProcessMessageAsync(msg!));
                if (tasks.Count >= _maxDegreeOfParallelism)
                {
                    await Task.WhenAny(tasks);
                    tasks = tasks.Where(t => !t.IsCompleted).ToList();
                }
                await Task.Delay(_throttleDelayMs);
            }
            await Task.Delay(500);
        }
    }

    private async Task ProcessMessageAsync(Message msg)
    {
        try
        {
            await _repo.UpdateStatusAsync(msg.Id, MessageStatus.Processing, msg.RetryCount);
            await SimulateExternalApi(msg);
            await _repo.UpdateStatusAsync(msg.Id, MessageStatus.Processed, msg.RetryCount);
            _logger.Information("Processed message {Id}", msg.Id);
        }
        catch
        {
            msg.RetryCount++;
            if (msg.RetryCount >= _maxRetry)
            {
                await _repo.UpdateStatusAsync(msg.Id, MessageStatus.DeadLetter, msg.RetryCount);
                _logger.Error("Message {Id} moved to DeadLetter after {Count} retries", msg.Id, msg.RetryCount);
            }
            else
            {
                await _repo.UpdateStatusAsync(msg.Id, MessageStatus.Failed, msg.RetryCount);
                _queue.Enqueue(msg);
                _logger.Warning("Message {Id} failed, retry {Count}", msg.Id, msg.RetryCount);
            }
        }
    }

    private async Task SimulateExternalApi(Message msg)
    {
        var rand = new Random();
        await Task.Delay(rand.Next(200, 1000));
        if (rand.NextDouble() < 0.2)
            throw new Exception("Simulated external service failure");
    }


}