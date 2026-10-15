
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EventsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Events
    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var events = await _context.Events
            .AsNoTracking()
            .Select(e => new
            {
                e.EventId,
                e.AccountId,
                e.EventName,
                e.EventDescription,
                e.EventDate,
                e.Location,
                e.EventType,
                e.BannerImageUrl
            })
            .ToListAsync();

        return Ok(events);
    }

    // GET: api/Events/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetEvent(int id)
    {
        var eventItem = await _context.Events
            .AsNoTracking()
            .Where(e => e.EventId == id)
            .Select(e => new
            {
                e.EventId,
                e.AccountId,
                e.EventName,
                e.EventDescription,
                e.EventDate,
                e.Location,
                e.EventType,
                e.BannerImageUrl
            })
            .FirstOrDefaultAsync();

        if (eventItem == null)
            return NotFound(new { message = "Event not found." });

        return Ok(eventItem);
    }

    // POST: api/Events
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> CreateEvent(Event request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        request.EventId = 0;
        request.AccountId = organiserId.Value;



        _context.Events.Add(request);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEvent), new { id = request.EventId },
            new { message = "Event created successfully.", eventId = request.EventId });


    }

    // PUT: api/Events/5
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateEvent(int id, Event request)
    {
        if (id != request.EventId)
            return BadRequest(new { message = "Event ID mismatch." });

        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        var existingEvent = await _context.Events.FindAsync(id);

        if (existingEvent == null)
            return NotFound(new { message = "Event not found." });

        if (existingEvent.AccountId != organiserId.Value)
            return Forbid();

        existingEvent.EventName = request.EventName;
        existingEvent.EventDescription = request.EventDescription;
        existingEvent.EventDate = request.EventDate;
        existingEvent.Location = request.Location;
        existingEvent.EventType = request.EventType;
        existingEvent.BannerImageUrl = request.BannerImageUrl;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Event updated successfully." });
    }

    // DELETE: api/Events/5
    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        var eventItem = await _context.Events.FindAsync(id);

        if (eventItem == null)
            return NotFound(new { message = "Event not found." });

        if (eventItem.AccountId != organiserId.Value)
            return Forbid();

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Event deleted successfully." });
    }
}


