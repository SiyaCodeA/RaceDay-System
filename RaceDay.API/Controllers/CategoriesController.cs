
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly RaceDayContext _context;

    public CategoriesController(RaceDayContext context)
    {
        _context = context;
    }

    // GET: api/Categories/event/1
    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetEventCategories(int eventId)
    {
        var eventExists = await _context.Events
            .AnyAsync(e => e.EventId == eventId);

        if (!eventExists)
            return NotFound(new { message = "Event not found." });

        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.EventId == eventId)
            .OrderBy(c => c.CategoryName)
            .Select(c => new
            {
                c.CategoryId,
                c.EventId,
                c.CategoryName,
                c.CategoryDescription,
                c.CategoryType,
                c.MinAge,
                c.MaxAge,
                c.DistanceKm
            })
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/Categories/1
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(c => c.CategoryId == id)
            .Select(c => new
            {
                c.CategoryId,
                c.EventId,
                c.CategoryName,
                c.CategoryDescription,
                c.CategoryType,
                c.MinAge,
                c.MaxAge,
                c.DistanceKm
            })
            .FirstOrDefaultAsync();

        if (category == null)
            return NotFound(new { message = "Category not found." });

        return Ok(category);
    }

    // POST: api/Categories
    [Authorize(Roles = "Organiser")]
    [HttpPost]
    public async Task<IActionResult> CreateCategory(Category request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.CategoryName) ||
            string.IsNullOrWhiteSpace(request.CategoryType))
        {
            return BadRequest(new
            {
                message = "Category name and category type are required."
            });
        }

        if (request.MinAge < 0 ||
            request.MaxAge < 0 ||
            (request.MinAge.HasValue &&
             request.MaxAge.HasValue &&
             request.MinAge > request.MaxAge) ||
            request.DistanceKm < 0)
        {
            return BadRequest(new
            {
                message = "Please provide valid age limits and distance."
            });
        }

        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        var eventItem = await _context.Events
            .FirstOrDefaultAsync(e => e.EventId == request.EventId);

        if (eventItem == null)
            return NotFound(new { message = "Event not found." });

        if (eventItem.AccountId != organiserId.Value)
            return Forbid();

        var categoryName = request.CategoryName.Trim();

        var duplicate = await _context.Categories.AnyAsync(c =>
            c.EventId == request.EventId &&
            c.CategoryName == categoryName);

        if (duplicate)
        {
            return Conflict(new
            {
                message = "This category already exists for this event."
            });
        }

        var category = new Category
        {
            EventId = request.EventId,
            CategoryName = categoryName,
            CategoryDescription = request.CategoryDescription,
            CategoryType = request.CategoryType.Trim(),
            MinAge = request.MinAge,
            MaxAge = request.MaxAge,
            DistanceKm = request.DistanceKm
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryId },
            new
            {
                message = "Category created successfully.",
                categoryId = category.CategoryId
            });
    }

    // PUT: api/Categories/1
    [Authorize(Roles = "Organiser")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateCategory(
        int id, Category request)
    {
        if (id != request.CategoryId)
        {
            return BadRequest(new
            {
                message = "Category ID mismatch."
            });
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(request.CategoryName) ||
            string.IsNullOrWhiteSpace(request.CategoryType))
        {
            return BadRequest(new
            {
                message = "Category name and category type are required."
            });
        }

        if (request.MinAge < 0 ||
            request.MaxAge < 0 ||
            (request.MinAge.HasValue &&
             request.MaxAge.HasValue &&
             request.MinAge > request.MaxAge) ||
            request.DistanceKm < 0)
        {
            return BadRequest(new
            {
                message = "Please provide valid age limits and distance."
            });
        }

        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        var category = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound(new { message = "Category not found." });

        if (category.Event.AccountId != organiserId.Value)
            return Forbid();

        var categoryName = request.CategoryName.Trim();

        var duplicate = await _context.Categories.AnyAsync(c =>
            c.EventId == category.EventId &&
            c.CategoryName == categoryName &&
            c.CategoryId != id);

        if (duplicate)
        {
            return Conflict(new
            {
                message = "This category name is already in use for this event."
            });
        }

        category.CategoryName = categoryName;
        category.CategoryDescription = request.CategoryDescription;
        category.CategoryType = request.CategoryType.Trim();
        category.MinAge = request.MinAge;
        category.MaxAge = request.MaxAge;
        category.DistanceKm = request.DistanceKm;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category updated successfully."
        });
    }

    // DELETE: api/Categories/1
    [Authorize(Roles = "Organiser")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var organiserId = HttpContext.Session.GetInt32("AccountId");

        if (!organiserId.HasValue)
            return Unauthorized();

        var category = await _context.Categories
            .Include(c => c.Event)
            .FirstOrDefaultAsync(c => c.CategoryId == id);

        if (category == null)
            return NotFound(new { message = "Category not found." });

        if (category.Event.AccountId != organiserId.Value)
            return Forbid();

        var hasEntries = await _context.Entries.AnyAsync(e =>
            e.EventId == category.EventId &&
            e.CategoryId == category.CategoryId);

        if (hasEntries)
        {
            return Conflict(new
            {
                message = "This category cannot be deleted because participants have already entered it."
            });
        }

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category deleted successfully."
        });
    }
}
