using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Services;
using RaceDay.MVC.ViewModels;

namespace RaceDay.MVC.Controllers;

public class AuthController : Controller
{
    private readonly RaceDayApiClient _apiClient;

    public AuthController(RaceDayApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _apiClient.PostAsync(
                "api/Auth/login",
                new
                {
                    model.Email,
                    model.Password
                });

            var responseBody =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(model);
            }

            using var document = JsonDocument.Parse(responseBody);
            var data = document.RootElement;

            var accountId = data.GetProperty("accountId").GetInt32();
            var firstName = data.GetProperty("firstName").GetString() ?? "";
            var role = data.GetProperty("role").GetString() ?? "";

            if (role != "Organiser" && role != "Participant")
            {
                _apiClient.ClearApiSession();

                ModelState.AddModelError(
                    string.Empty,
                    "Your account has an invalid role.");

                return View(model);
            }

            HttpContext.Session.SetInt32("AccountId", accountId);
            HttpContext.Session.SetString("FirstName", firstName);
            HttpContext.Session.SetString("UserRole", role);

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to RaceDay. Please try again.");

            return View(model);
        }
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _apiClient.PostAsync(
                "api/Auth/register",
                new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password,
                    model.PhoneNumber,
                    Role = "Participant"
                });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. This email may already be registered.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful. You can now log in.";

            return RedirectToAction(nameof(Login));
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to connect to RaceDay. Please try again.");

            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _apiClient.PostAsync("api/Auth/logout", new { });
        }
        catch (HttpRequestException)
        {
            // Clear the local session even if the API is unavailable.
        }

        _apiClient.ClearApiSession();
        HttpContext.Session.Clear();

        return RedirectToAction(nameof(Login));
    }
}