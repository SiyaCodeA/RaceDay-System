using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Models;
using System.Diagnostics;

namespace RaceDay.MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role == "Participant")
            {
                return RedirectToAction("Index", "Participant");
            }

            if (role == "Organiser")
            {
                return RedirectToAction("Index", "Organiser");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
