using Xunit;
using MessagingQueueApp.Models;
using MessagingQueueApp.Services;
using System;
using System.Threading.Tasks;

public class MessageRepositoryTests
{
    [Fact]
    public async Task AddAndGetMessage_ShouldReturnSameMessage()
    {
        var repo = new MessageRepository("Host=localhost;Database=msgqueue;Username=postgres;Password=postgres");
        var msg = new Message
        {
            Id = Guid.NewGuid(),
            Type = MessageType.Email,
            Recipient = "test@example.com",
            Content = "Test",
            Status = MessageStatus.Pending,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            Priority = 1
        };

        await repo.AddAsync(msg);
        var fetched = await repo.GetAsync(msg.Id);

        Assert.NotNull(fetched);
        Assert.Equal(msg.Content, fetched.Content);
    }
}