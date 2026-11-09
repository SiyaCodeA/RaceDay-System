using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers;

public class ParticipantController : Controller
{
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
}