using System.Net;
using System.Net.Http.Json;
using MessagingQueueApp.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Xunit;

namespace MessagingQueueApp.Tests;

public class MessagesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public MessagesControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Post_EnqueueMessage_ReturnsAccepted()
    {
        var message = new Message
        {
            Recipient = "test@example.com",
            Content = "Hello test!",
            Type = MessageType.Email
        };

        var response = await _client.PostAsJsonAsync("/api/messages", message);

        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }

    [Fact]
    public async Task GetAllMessages_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/messages/all");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetStats_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/messages/stats");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetTypeStats_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/messages/type-stats");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetPendingMessages_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/messages/pending");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateStatus_ReturnsNoContent()
    {
        // First enqueue a message
        var message = new Message
        {
            Recipient = "update@example.com",
            Content = "Update me",
            Type = MessageType.SMS
        };

        var enqueueResponse = await _client.PostAsJsonAsync("/api/messages", message);
        var result = await enqueueResponse.Content.ReadFromJsonAsync<EnqueueResponse>();
        var messageId = result?.Id ?? Guid.Empty;

        // Update status
        var statusUpdate = new
        {
            status = MessageStatus.Processed,
            retryCount = 1
        };

        var updateResponse = await _client.PostAsJsonAsync($"/api/messages/{messageId}/status", statusUpdate);
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);
    }

    private class EnqueueResponse
    {
        public Guid Id { get; set; }
    }
}
