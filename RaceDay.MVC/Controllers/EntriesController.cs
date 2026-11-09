
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers;

public class EntriesController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public EntriesController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool IsParticipant()
    {
        return HttpContext.Session.GetString("UserRole")
            == "Participant";
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int eventId, int categoryId)
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.PostAsync(
                "api/Entries",
                new
                {
                    EventId = eventId,
                    CategoryId = categoryId
                });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "You have successfully entered this event. Your entry is pending confirmation.";
            }
            else if (response.StatusCode == HttpStatusCode.Conflict)
            {
                TempData["ErrorMessage"] =
                    "You have already entered this event.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    await response.Content.ReadAsStringAsync();
            }

            return RedirectToAction(
                "Details", "Events", new { id = eventId });
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Unable to connect to the RaceDay service.";

            return RedirectToAction(
                "Details", "Events", new { id = eventId });
        }
    }

    [HttpGet]
    public async Task<IActionResult> MyEntries()
    {
        if (!IsParticipant())
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                "api/Entries/mine");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve your event entries.";

                return View(new List<EntryViewModel>());
            }

            var entries = await response.Content
                .ReadFromJsonAsync<List<EntryViewModel>>();

            return View(entries ?? new List<EntryViewModel>());
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage =
                "Could not connect to the RaceDay service.";

            return View(new List<EntryViewModel>());
        }
    }



    [HttpGet]
    public async Task<IActionResult> ForEvent(int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                $"api/Entries/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "Unable to retrieve event entries.";

                return RedirectToAction("Index", "Events");
            }

            var entries = await response.Content
                .ReadFromJsonAsync<List<OrganiserEntryViewModel>>();

            ViewBag.EventId = eventId;

            return View(entries ?? new List<OrganiserEntryViewModel>());
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Could not connect to the RaceDay service.";

            return RedirectToAction("Index", "Events");
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirm(int entryId, int eventId)
    {
        if (HttpContext.Session.GetString("UserRole") != "Organiser")
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.PutAsync(
                $"api/Entries/{entryId}/confirm",
                new { });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Participant entry confirmed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    "The participant entry could not be confirmed.";
            }
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Could not connect to the RaceDay service.";
        }

        return RedirectToAction(
            "ForEvent", new { eventId });
    }

}

