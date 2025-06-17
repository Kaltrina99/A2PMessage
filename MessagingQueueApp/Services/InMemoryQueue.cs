using System.Collections.Concurrent;
using MessagingQueueApp.Models;

namespace MessagingQueueApp.Services;

public class InMemoryQueue : IQueueProvider
{
    private readonly ConcurrentQueue<Message> _queue = new();

    public void Enqueue(Message msg) => _queue.Enqueue(msg);

    public bool TryDequeue(out Message? msg) => _queue.TryDequeue(out msg);

    public int Count => _queue.Count;
}