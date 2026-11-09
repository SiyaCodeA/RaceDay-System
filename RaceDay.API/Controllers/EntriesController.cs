
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EntriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public EntriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Entries/mine
    // Allows a participant to view their own entries.
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyEntries()
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        var entries = await _context.Entries
            .Where(e => e.AccountId == accountId)
            .Select(e => new
            {
                e.EntryId,
                e.AccountId,
                e.EventId,
                e.CategoryId,
                e.EntryDate,
                e.EntryStatus,
                EventName = e.Event.EventName,
                EventDate = e.Event.EventDate,
                Location = e.Event.Location,
                CategoryName = e.Category.CategoryName,
                CategoryType = e.Category.CategoryType
            })
            .ToListAsync();

        return Ok(entries);
    }

    // POST: api/Entries
    // Allows a participant to enter an event.
    [HttpPost]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> CreateEntry(
        [FromBody] CreateEntryRequest request)
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        if (request.EventId <= 0 || request.CategoryId <= 0)
            return BadRequest("Select a valid event and category.");

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(e =>
                e.EventId == request.EventId);

        if (eventItem == null)
            return NotFound("Event not found.");

        var category = await _context.Categories
            .FirstOrDefaultAsync(c =>
                c.CategoryId == request.CategoryId &&
                c.EventId == request.EventId);

        if (category == null)
            return BadRequest(
                "The selected category does not belong to this event.");

        bool alreadyEntered = await _context.Entries
            .AnyAsync(e =>
                e.AccountId == accountId &&
                e.EventId == request.EventId);

        if (alreadyEntered)
            return Conflict(
                "You have already entered this event.");

        var entry = new Entry
        {
            AccountId = accountId,
            EventId = request.EventId,
            CategoryId = request.CategoryId,
            EntryDate = DateTime.Now,
            EntryStatus = "Pending"
        };

        _context.Entries.Add(entry);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Event entry created successfully.",
            entry.EntryId,
            entry.EventId,
            entry.CategoryId,
            entry.EntryStatus
        });
    }

    // GET: api/Entries/event/1
    // Allows an organiser to view entries for their own event.
    [HttpGet("event/{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetEventEntries(int eventId)
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == eventId);

        if (eventItem == null)
            return NotFound("Event not found.");

        if (eventItem.AccountId != accountId)
            return Forbid();

        var entries = await _context.Entries
            .Where(e => e.EventId == eventId)
            .Select(e => new
            {
                e.EntryId,
                e.AccountId,
                ParticipantName =
                    e.Account.FirstName + " " + e.Account.LastName,
                e.CategoryId,
                CategoryName = e.Category.CategoryName,
                e.EntryDate,
                e.EntryStatus
            })
            .ToListAsync();

        return Ok(entries);
    }

    [HttpPut("{entryId:int}/confirm")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> ConfirmEntry(int entryId)
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        var entry = await _context.Entries
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EntryId == entryId);

        if (entry == null)
            return NotFound("Entry not found.");

        if (entry.Event.AccountId != accountId)
            return Forbid();

        entry.EntryStatus = "Confirmed";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Entry confirmed successfully.",
            entry.EntryId,
            entry.EntryStatus
        });
    }

}

public class CreateEntryRequest
{
    public int EventId { get; set; }

    public int CategoryId { get; set; }
}