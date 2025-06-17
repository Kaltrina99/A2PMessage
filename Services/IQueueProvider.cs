using MessagingQueueApp.Models;

namespace MessagingQueueApp.Services;

public interface IQueueProvider
{
    void Enqueue(Message msg);
    bool TryDequeue(out Message? msg);
    int Count { get; }
}