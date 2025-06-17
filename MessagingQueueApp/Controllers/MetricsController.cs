using MessagingQueueApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingQueueApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IQueueProvider _queue;

    public MetricsController(IQueueProvider queue)
    {
        _queue = queue;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new { queueDepth = _queue.Count });
    }
}