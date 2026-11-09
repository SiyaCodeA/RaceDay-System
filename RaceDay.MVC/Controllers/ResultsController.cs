using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers;

public class ResultsController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public ResultsController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private string? UserRole =>
        HttpContext.Session.GetString("UserRole");

    // Organiser results landing page
    public IActionResult Index()
    {
        if (UserRole != "Organiser")
            return RedirectToAction("Login", "Auth");

        return RedirectToAction("Index", "Events");
    }

    // View results and entries for one event
    [HttpGet]
    public async Task<IActionResult> Event(int eventId)
    {
        if (UserRole != "Organiser")
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                $"api/Results/event/{eventId}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] =
                    "Unable to retrieve event results.";

                return RedirectToAction("Index", "Events");
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<OrganiserResultViewModel>>();

            ViewBag.EventId = eventId;

            return View(results ?? new List<OrganiserResultViewModel>());
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Could not connect to the RaceDay service.";

            return RedirectToAction("Index", "Events");
        }
    }

    // Save or update a participant's result
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(SaveResultViewModel model)
    {
        if (UserRole != "Organiser")
            return RedirectToAction("Login", "Auth");

        if (string.IsNullOrWhiteSpace(model.FinishTime) &&
            !model.FinishPosition.HasValue)
        {
            TempData["ErrorMessage"] =
                "Enter a finish time or finish position.";

            return RedirectToAction(
                "Event", new { eventId = model.EventId });
        }

        try
        {
            var response = await _apiClient.PutAsync(
                $"api/Results/entry/{model.EntryId}",
                new
                {
                    model.FinishTime,
                    model.FinishPosition
                });

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] =
                    "Participant result saved successfully.";
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();

                TempData["ErrorMessage"] =
                    string.IsNullOrWhiteSpace(error)
                    ? "Unable to save the participant result."
                    : error;
            }
        }
        catch (HttpRequestException)
        {
            TempData["ErrorMessage"] =
                "Could not connect to the RaceDay service.";
        }

        return RedirectToAction(
            "Event", new { eventId = model.EventId });
    }

    // Participant views their own results
    [HttpGet]
    public async Task<IActionResult> MyResults()
    {
        if (UserRole != "Participant")
            return RedirectToAction("Login", "Auth");

        try
        {
            var response = await _apiClient.GetAsync(
                "api/Results/mine");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve your results.";

                return View(new List<ParticipantResultViewModel>());
            }

            var results = await response.Content
                .ReadFromJsonAsync<List<ParticipantResultViewModel>>();

            return View(results ?? new List<ParticipantResultViewModel>());
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage =
                "Could not connect to the RaceDay service.";

            return View(new List<ParticipantResultViewModel>());
        }
    }
}