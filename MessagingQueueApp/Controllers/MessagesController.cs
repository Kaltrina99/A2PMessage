using MessagingQueueApp.Models;
using MessagingQueueApp.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace MessagingQueueApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly Producer _producer;
    private readonly MessageRepository _repo;
    private readonly ILogger _logger;

    public MessagesController(Producer producer, MessageRepository repo, ILogger logger)
    {
        _producer = producer;
        _repo = repo;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Enqueue([FromBody] Message msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Recipient) || string.IsNullOrWhiteSpace(msg.Content))
            return BadRequest("Recipient and content required.");

        var id = await _producer.ProduceAsync(msg);
        return Accepted(new { id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var msg = await _repo.GetAsync(id);
        return msg == null ? NotFound() : Ok(msg);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var all = await _repo.GetAllAsync();
        return Ok(all);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var statusStats = await _repo.GetStatsByStatusAsync();
        return Ok(statusStats);
    }

    [HttpGet("type-stats")]
    public async Task<IActionResult> TypeStats()
    {
        var typeStats = await _repo.GetStatsByTypeAsync();
        return Ok(typeStats);
    }
    // Get pending messages
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending()
    {
        var pendingMessages = await _repo.GetByStatusAsync(MessageStatus.Pending);
        return Ok(pendingMessages);
    }

    // Update message status manually
    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] MessageStatusUpdateRequest request)
    {
        await _repo.UpdateStatusAsync(id, request.Status, request.RetryCount);
        return NoContent();
    }

    public class MessageStatusUpdateRequest
    {
        public MessageStatus Status { get; set; }
        public int RetryCount { get; set; } = 0;
    }

}
