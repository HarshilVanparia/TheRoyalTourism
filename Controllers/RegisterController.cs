using Microsoft.AspNetCore.Mvc;

namespace TheRoyalTourism.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult RegisterPage()
        {
            return View();
        }
        public IActionResult LoginPage()
        {
            return View();
        }
    }
}
