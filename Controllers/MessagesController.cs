using MessagingQueueApp.Models;
using MessagingQueueApp.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace MessagingQueueApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessagesController : ControllerBase
{
    private readonly IQueueProvider _queue;
    private readonly MessageRepository _repo;
    private readonly ILogger _logger;

    public MessagesController(IQueueProvider queue, MessageRepository repo, ILogger logger)
    {
        _queue = queue;
        _repo = repo;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Enqueue([FromBody] Message msg)
    {
        if (string.IsNullOrWhiteSpace(msg.Recipient) || string.IsNullOrWhiteSpace(msg.Content))
            return BadRequest("Recipient and content required.");
        msg.Id = Guid.NewGuid();
        msg.Status = MessageStatus.Pending;
        msg.CreatedAt = DateTime.UtcNow;
        msg.RetryCount = 0;
        msg.Priority = msg.Priority == 0 ? 1 : msg.Priority;
        await _repo.AddAsync(msg);
        _queue.Enqueue(msg);
        _logger.Information("Enqueued message {Id}", msg.Id);
        return Accepted(new { msg.Id });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var msg = await _repo.GetAsync(id);
        if (msg == null) return NotFound();
        return Ok(msg);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats()
    {
        var byStatus = await _repo.GetStatsByStatusAsync();
        return Ok(byStatus);
    }
}