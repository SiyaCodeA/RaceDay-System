
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers;

public class ParticipantController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public ParticipantController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("UserRole");

        if (role != "Participant")
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.FirstName =
            HttpContext.Session.GetString("FirstName");

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var role = HttpContext.Session.GetString("UserRole");

        if (role != "Participant")
        {
            return RedirectToAction("Login", "Auth");
        }

        try
        {
            var response = await _apiClient.GetAsync(
                "api/Auth/profile");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve your profile. Please try again.";

                return View(new ParticipantProfileViewModel());
            }

            var profile = await response.Content
                .ReadFromJsonAsync<ParticipantProfileViewModel>();

            if (profile == null)
            {
                ViewBag.ErrorMessage =
                    "Your profile information could not be loaded.";

                return View(new ParticipantProfileViewModel());
            }

            return View(profile);
        }
        catch (HttpRequestException)
        {
            ViewBag.ErrorMessage =
                "Could not connect to the RaceDay service.";

            return View(new ParticipantProfileViewModel());
        }
    }
}
