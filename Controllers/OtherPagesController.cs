using Microsoft.AspNetCore.Mvc;

namespace TheRoyalTourism.Controllers
{
    public class OtherPagesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Aboutus()
        {
            return View();
        }
    }
}
