using System.Net.Http.Json;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers;

public class CategoriesController : Controller
{
    private readonly RaceDayApiClient _api;

    public CategoriesController(RaceDayApiClient api)
    {
        _api = api;
    }

    private bool IsOrganiser =>
        HttpContext.Session.GetString("UserRole") == "Organiser";

    private int AccountId =>
        HttpContext.Session.GetInt32("AccountId") ?? 0;

    private IActionResult OrganiserLogin()
    {
        return RedirectToAction("Login", "Auth");
    }

    private async Task<EventViewModel?> GetOwnedEvent(int eventId)
    {
        using var response =
            await _api.GetAsync($"api/Events/{eventId}");

        if (!response.IsSuccessStatusCode)
            return null;

        var eventItem =
            await response.Content.ReadFromJsonAsync<EventViewModel>();

        if (eventItem == null || eventItem.AccountId != AccountId)
            return null;

        return eventItem;
    }

    private async Task<CategoryViewModel?> GetCategory(int id)
    {
        using var response =
            await _api.GetAsync($"api/Categories/{id}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<CategoryViewModel>();
    }

    // LIST CATEGORIES FOR AN EVENT
    public async Task<IActionResult> Index(int eventId)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var eventItem = await GetOwnedEvent(eventId);

        if (eventItem == null)
            return NotFound("Event not found or you do not own this event.");

        using var response =
            await _api.GetAsync($"api/Categories/event/{eventId}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Could not load categories.";
            return View(new List<CategoryViewModel>());
        }

        var categories =
            await response.Content.ReadFromJsonAsync<List<CategoryViewModel>>()
            ?? new List<CategoryViewModel>();

        ViewBag.EventName = eventItem.EventName;
        ViewBag.EventId = eventId;

        return View(categories);
    }

    // CREATE CATEGORY
    public async Task<IActionResult> Create(int eventId)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var eventItem = await GetOwnedEvent(eventId);

        if (eventItem == null)
            return NotFound();

        return View(new CategoryViewModel
        {
            EventId = eventId,
            EventName = eventItem.EventName,
            CategoryType = "Age"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryViewModel model)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var eventItem = await GetOwnedEvent(model.EventId);

        if (eventItem == null)
            return NotFound();

        model.EventName = eventItem.EventName;

        if (!ModelState.IsValid)
            return View(model);

        var data = new
        {
            model.EventId,
            model.CategoryName,
            model.CategoryDescription,
            model.CategoryType,
            model.MinAge,
            model.MaxAge,
            model.DistanceKm
        };

        using var response =
            await _api.PostAsync("api/Categories", data);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                await response.Content.ReadAsStringAsync());

            return View(model);
        }

        TempData["Success"] = "Category created successfully.";

        return RedirectToAction(
            nameof(Index), new { eventId = model.EventId });
    }

    // EDIT CATEGORY
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var category = await GetCategory(id);

        if (category == null)
            return NotFound();

        var eventItem = await GetOwnedEvent(category.EventId);

        if (eventItem == null)
            return NotFound();

        category.EventName = eventItem.EventName;

        return View(category);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id, CategoryViewModel model)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        if (id != model.CategoryId)
            return BadRequest();

        var eventItem = await GetOwnedEvent(model.EventId);

        if (eventItem == null)
            return NotFound();

        model.EventName = eventItem.EventName;

        if (!ModelState.IsValid)
            return View(model);

        var data = new
        {
            model.CategoryId,
            model.EventId,
            model.CategoryName,
            model.CategoryDescription,
            model.CategoryType,
            model.MinAge,
            model.MaxAge,
            model.DistanceKm
        };

        using var response =
            await _api.PutAsync($"api/Categories/{id}", data);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                "",
                await response.Content.ReadAsStringAsync());

            return View(model);
        }

        TempData["Success"] = "Category updated successfully.";

        return RedirectToAction(
            nameof(Index), new { eventId = model.EventId });
    }

    // DELETE CATEGORY
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var category = await GetCategory(id);

        if (category == null)
            return NotFound();

        var eventItem = await GetOwnedEvent(category.EventId);

        if (eventItem == null)
            return NotFound();

        category.EventName = eventItem.EventName;

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(
        int id, int eventId)
    {
        if (!IsOrganiser)
            return OrganiserLogin();

        var eventItem = await GetOwnedEvent(eventId);

        if (eventItem == null)
            return NotFound();

        using var response =
            await _api.DeleteAsync($"api/Categories/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] =
                "Could not delete this category. It may already have participant entries.";

            return RedirectToAction(
                nameof(Index), new { eventId });
        }

        TempData["Success"] = "Category deleted successfully.";

        return RedirectToAction(nameof(Index), new { eventId });
    }
}