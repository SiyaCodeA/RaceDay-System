using Microsoft.AspNetCore.Mvc;

namespace RaceDay.MVC.Controllers;

public class OrganiserController : Controller
{
    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("UserRole");

        if (role != "Organiser")
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.FirstName =
            HttpContext.Session.GetString("FirstName");

        return View();
    }
}