using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;

namespace TheRoyalTourism.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult RegisterPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegisterUser(string fullname, string email, string pnumber, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (fullname, email, pnumber, password, role) VALUES (@fullname, @email, @pnumber, @password, 'user')";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@fullname", fullname);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pnumber", pnumber);
                    cmd.Parameters.AddWithValue("@password", password);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["msg"] = "Registration successful! Please log in.";
                        return RedirectToAction("RegisterPage");
                    }
                    else
                    {
                        TempData["error"] = "Registration failed. Try again.";
                        return RedirectToAction("RegisterPage");
                    }
                }
            }
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT fullname, email, pnumber, role FROM users WHERE email = @email AND password = @password";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    HttpContext.Session.SetString("UserName", reader["fullname"].ToString());
                    HttpContext.Session.SetString("UserEmail", reader["email"].ToString());
                    HttpContext.Session.SetString("Pnumber", reader["pnumber"].ToString());
                    HttpContext.Session.SetString("UserRole", reader["role"].ToString());

                    string role = reader["role"].ToString();

                    if (role == "user")
                        return RedirectToAction("Domestic", "Destination");

                    if (role == "admin")
                        return RedirectToAction("Dashboard", "Admin");
                }

                ViewBag.ErrorMessage = "Invalid email or password.";
                reader.Close();
                conn.Close();
                return View("LoginPage");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }
    }
}
