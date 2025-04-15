using Microsoft.AspNetCore.Mvc;
using EventProcessor.Data;
using Microsoft.EntityFrameworkCore;

namespace EventProcessor.Controllers;

[ApiController]
[Route("incident")]
public class IncidentController : ControllerBase
{
    private readonly AppDbContext _context;

    public IncidentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetIncidents()
    {
        var incidents = await _context.Incidents
            .Include(i => i.Events)
            .ToListAsync();

        return Ok(incidents);
    }
}