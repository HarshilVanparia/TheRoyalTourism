using Microsoft.AspNetCore.Mvc;

namespace TheRoyalTourism.Controllers
{
    public class DestinationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Domestic()
        {
            return View();
        }

        public IActionResult International()
        {
            return View();
        }

        public IActionResult AnyFestival()
        {
            return View();
        }
    }
}
