using Microsoft.AspNetCore.Mvc;
using EventShared.Models;
using EventProcessor.Services;

namespace EventProcessor.Controllers;

[ApiController]
[Route("event")]
[Tags("EventProcessor")]
public class EventController : ControllerBase
{
    private readonly IEventHandlerService _eventHandler;

    public EventController(IEventHandlerService eventHandler)
    {
        _eventHandler = eventHandler;
    }

    [HttpPost("receive")]
    public async Task<IActionResult> ReceiveEvent([FromBody] Event model)
    {
        await _eventHandler.HandleEventAsync(model);
        return Ok();
    }
    
}
