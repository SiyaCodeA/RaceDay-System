
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers;

public class EventsController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public EventsController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool IsOrganiser()
    {
        return HttpContext.Session.GetString("UserRole") == "Organiser";
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var response = await _apiClient.GetAsync("api/Events");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage = "Unable to retrieve events.";
                return View(new List<EventViewModel>());
            }

            var events = await response.Content
                .ReadFromJsonAsync<List<EventViewModel>>();

            ViewBag.IsOrganiser = IsOrganiser();
            ViewBag.AccountId =
                HttpContext.Session.GetInt32("AccountId");

            return View(events ?? new List<EventViewModel>());
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage =
                "Could not connect to the RaceDay service.";

            return View(new List<EventViewModel>());
        }
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        try
        {
            var eventResponse = await _apiClient.GetAsync(
                $"api/Events/{id}");

            if (!eventResponse.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await eventResponse.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            var categoryResponse = await _apiClient.GetAsync(
                $"api/Categories/event/{id}");

            if (!categoryResponse.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "The event was found, but its categories could not be loaded.";

                return View(new EventDetailsViewModel
                {
                    Event = eventItem
                });
            }

            var categories = await categoryResponse.Content
                .ReadFromJsonAsync<List<CategoryViewModel>>();

            ViewBag.IsParticipant =
                HttpContext.Session.GetString("UserRole") == "Participant";

            ViewBag.IsOrganiser = IsOrganiser();

            return View(new EventDetailsViewModel
            {
                Event = eventItem,
                Categories = categories ?? new List<CategoryViewModel>()
            });
        }
        catch (HttpRequestException)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        return View(new EventViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        EventViewModel model,
        IFormFile? bannerImage)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        if (model.EventDate < DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(
                nameof(model.EventDate),
                "The event date cannot be in the past.");
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _apiClient.PostAsync(
                "api/Events", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event could not be created.");

                return View(model);
            }

            using var document = JsonDocument.Parse(
                await response.Content.ReadAsStringAsync());

            var eventId = document.RootElement
                .GetProperty("eventId").GetInt32();

            if (bannerImage != null && bannerImage.Length > 0)
            {
                var uploadResponse =
                    await _apiClient.UploadEventBannerAsync(
                        eventId, bannerImage);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        "The event was created, but the banner upload failed. You can try again later.";

                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["SuccessMessage"] =
                "Event created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to the RaceDay service.");

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                $"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await response.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            if (eventItem.AccountId !=
                HttpContext.Session.GetInt32("AccountId"))
            {
                return Forbid();
            }

            return View(eventItem);
        }
        catch (HttpRequestException)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        EventViewModel model,
        IFormFile? bannerImage)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        if (id != model.EventId)
            return BadRequest();

        if (model.EventDate < DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError(
                nameof(model.EventDate),
                "The event date cannot be in the past.");
        }

        if (!ModelState.IsValid)
            return View(model);

        try
        {
            // Preserve the existing banner if no replacement is selected.
            var existingResponse = await _apiClient.GetAsync(
                $"api/Events/{id}");

            if (!existingResponse.IsSuccessStatusCode)
                return NotFound();

            var existingEvent = await existingResponse.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (existingEvent == null)
                return NotFound();

            if (existingEvent.AccountId !=
                HttpContext.Session.GetInt32("AccountId"))
            {
                return Forbid();
            }

            model.BannerImageUrl = existingEvent.BannerImageUrl;

            var response = await _apiClient.PutAsync(
                $"api/Events/{id}", model);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event could not be updated. Check that you own this event.");

                return View(model);
            }

            if (bannerImage != null && bannerImage.Length > 0)
            {
                var uploadResponse =
                    await _apiClient.UploadEventBannerAsync(
                        id, bannerImage);

                if (!uploadResponse.IsSuccessStatusCode)
                {
                    TempData["ErrorMessage"] =
                        "Event details were updated, but the banner upload failed.";

                    return RedirectToAction(nameof(Index));
                }
            }

            TempData["SuccessMessage"] =
                "Event updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to the RaceDay service.");

            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                $"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
                return NotFound();

            var eventItem = await response.Content
                .ReadFromJsonAsync<EventViewModel>();

            if (eventItem == null)
                return NotFound();

            if (eventItem.AccountId !=
                HttpContext.Session.GetInt32("AccountId"))
            {
                return Forbid();
            }

            return View(eventItem);
        }
        catch (HttpRequestException)
        {
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!IsOrganiser())
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.DeleteAsync(
                $"api/Events/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "The event could not be deleted.";

                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] =
                "Event deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Unable to connect to the RaceDay service.";

            return RedirectToAction(nameof(Index));
        }
    }
}
