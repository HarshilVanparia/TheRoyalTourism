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
<<<<<<< HEAD

        public IActionResult PackagesPage()
        {
            return View();
        }
=======
>>>>>>> 5b8ec4d2fd6707b56d0a5d1be240b231728d6cf2
    }
}
