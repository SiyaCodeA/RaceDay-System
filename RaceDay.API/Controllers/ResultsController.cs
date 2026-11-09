
using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResultsController : ControllerBase
{
    private readonly RaceDayContext _context;

    public ResultsController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Results/event/1
    // Organiser views results and entries for their own event.
    [HttpGet("event/{eventId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> GetEventResults(int eventId)
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

        var results = await _context.Entries
            .Where(e => e.EventId == eventId)
            .Select(e => new
            {
                e.EntryId,
                e.AccountId,
                ParticipantName =
                    e.Account.FirstName + " " + e.Account.LastName,
                e.CategoryId,
                CategoryName = e.Category.CategoryName,
                e.EntryStatus,
                e.EntryDate,
                ResultId = e.Result == null
                    ? (int?)null : e.Result.ResultId,
                FinishTime = e.Result == null ||
                    e.Result.FinishTime == null
                    ? null
                    : e.Result.FinishTime.Value.ToString("HH:mm:ss"),
                FinishPosition = e.Result == null
                    ? (int?)null : e.Result.FinishPosition
            })
            .ToListAsync();

        return Ok(results);
    }

    // GET: api/Results/mine
    // Participant views their own results.
    [HttpGet("mine")]
    [Authorize(Roles = "Participant")]
    public async Task<IActionResult> GetMyResults()
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        var results = await _context.Entries
            .Where(e => e.AccountId == accountId &&
                        e.Result != null)
            .Select(e => new
            {
                e.EntryId,
                e.EventId,
                EventName = e.Event.EventName,
                EventDate = e.Event.EventDate,
                Location = e.Event.Location,
                CategoryName = e.Category.CategoryName,
                FinishTime = e.Result!.FinishTime == null
                    ? null
                    : e.Result.FinishTime.Value.ToString("HH:mm:ss"),
                FinishPosition = e.Result!.FinishPosition
            })
            .ToListAsync();

        return Ok(results);
    }

    // PUT: api/Results/entry/1
    // Organiser records or updates a participant's result.
    [HttpPut("entry/{entryId:int}")]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> SaveResult(
        int entryId,
        [FromBody] SaveResultRequest request)
    {
        int accountId =
            HttpContext.Session.GetInt32("AccountId") ?? 0;

        if (accountId == 0)
            return Unauthorized();

        if (request.FinishPosition.HasValue &&
            request.FinishPosition.Value <= 0)
        {
            return BadRequest(
                "Finish position must be greater than zero.");
        }

        TimeOnly? finishTime = null;

        if (!string.IsNullOrWhiteSpace(request.FinishTime))
        {
            if (!TimeOnly.TryParse(
                request.FinishTime,
                CultureInfo.InvariantCulture,
                out var parsedTime))
            {
                return BadRequest(
                    "Enter a valid finish time, for example 00:35:42.");
            }

            finishTime = parsedTime;
        }

        if (!finishTime.HasValue &&
            !request.FinishPosition.HasValue)
        {
            return BadRequest(
                "Enter a finish time or finish position.");
        }

        var entry = await _context.Entries
            .Include(e => e.Event)
            .FirstOrDefaultAsync(e => e.EntryId == entryId);

        if (entry == null)
            return NotFound("Entry not found.");

        if (entry.Event.AccountId != accountId)
            return Forbid();

        var result = await _context.Results
            .FirstOrDefaultAsync(r => r.EntryId == entryId);

        if (result == null)
        {
            result = new Result
            {
                EntryId = entryId
            };

            _context.Results.Add(result);
        }

        result.FinishTime = finishTime;
        result.FinishPosition = request.FinishPosition;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Result saved successfully.",
            result.ResultId,
            result.EntryId,
            FinishTime = result.FinishTime?.ToString("HH:mm:ss"),
            result.FinishPosition
        });
    }
}

public class SaveResultRequest
{
    public string? FinishTime { get; set; }

    public int? FinishPosition { get; set; }
}
