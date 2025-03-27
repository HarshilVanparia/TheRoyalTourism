using Microsoft.AspNetCore.Mvc;

namespace TheRoyalTourism.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            return View();
        } 
        public IActionResult Forms()
        {
            return View();
        }
    }
}
