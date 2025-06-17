using System;

namespace MessagingQueueApp.Models;

public enum MessageType { SMS, Email, PushNotification }
public enum MessageStatus { Pending, Processing, Processed, Failed, DeadLetter }

public class Message
{
    public Guid Id { get; set; }
    public MessageType Type { get; set; }
    public string Recipient { get; set; }
    public string Content { get; set; }
    public MessageStatus Status { get; set; }
    public int RetryCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int Priority { get; set; }
}