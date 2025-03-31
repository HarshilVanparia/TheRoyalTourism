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
            if (HttpContext.Session.GetString("UserRole") != "admin")
            {
                return RedirectToAction("LoginPage", "Register");
            }
            return View();
        }
        public IActionResult Forms()
        {
            return View();
        }
        public IActionResult DataTables()
        {
            return View();
        }
        public IActionResult Packages()
        {
            return View();
        }
        public IActionResult Details()
        {
            return View();
        } 
        public IActionResult AdminProfile()
        {
            return View();
        }
    }
}
