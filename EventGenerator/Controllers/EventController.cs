using EventShared.Models;
using Microsoft.AspNetCore.Mvc;


namespace EventGenerator.Controllers;

[ApiController]
[Route("event")]
[Tags("EventGenerator")]
public class EventController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private static readonly Random _random = new();

    public EventController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("manual")]
    public async Task<IActionResult> GenerateManualEvent()
    {
        var newEvent = new Event
        {
            Type = (EventType)_random.Next(1, 4),
            Time = DateTime.UtcNow
        };

        var client = _httpClientFactory.CreateClient("ProcessorClient");

        try
        {
            var response = await client.PostAsJsonAsync("/event/receive", newEvent);
            return Ok(newEvent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("custom")]
    public async Task<IActionResult> GenerateCustomEvent([FromBody] Event customEvent)
    {
        customEvent.Time = DateTime.UtcNow;

        var client = _httpClientFactory.CreateClient("ProcessorClient");

        try
        {
            var response = await client.PostAsJsonAsync("/event/receive", customEvent);
            return Ok(customEvent);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
