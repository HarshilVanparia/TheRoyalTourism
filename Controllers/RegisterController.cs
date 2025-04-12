using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using TheRoyalTourism.Models;

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
        public IActionResult RegisterUser(UserModel user)
        {
            if (!ModelState.IsValid)
            {
                return View("RegisterPage", user); // Return form with validation errors
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", user.Email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["msg"] = "Email already exists!";
                        return RedirectToAction("RegisterPage");
                    }
                }

                // Insert user into the database
                string insertQuery = "INSERT INTO users (fullname, email, pnumber, password, role) VALUES (@FullName, @Email, @PNumber, @Password, 'user')";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.Fullname);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@PNumber", user.Pnumber);
                    cmd.Parameters.AddWithValue("@Password", user.Password); // Consider hashing password

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["msg"] = "Registration successful! Please log in.";
            return RedirectToAction("RegisterPage");
        }

        public IActionResult LoginPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model)
        {
            // Ensure model is valid before proceeding
            if (!ModelState.IsValid)
            {
                return View("LoginPage", model);
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "SELECT uid, fullname, email, pnumber, role FROM users WHERE email = @Email AND password = @Password";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Check if Email and Password are not null or empty
                    if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
                    {
                        ViewBag.ErrorMessage = "Email and Password are required.";
                        return View("LoginPage", model);
                    }

                    // Add parameters properly
                    cmd.Parameters.Add(new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 255) { Value = model.Email });
                    cmd.Parameters.Add(new SqlParameter("@Password", System.Data.SqlDbType.NVarChar, 255) { Value = model.Password });

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        HttpContext.Session.SetInt32("UserId", Convert.ToInt32(reader["uid"]));
                        HttpContext.Session.SetString("UserName", reader["fullname"].ToString());
                        HttpContext.Session.SetString("UserEmail", reader["email"].ToString());
                        HttpContext.Session.SetString("Pnumber", reader["pnumber"].ToString());
                        HttpContext.Session.SetString("UserRole", reader["role"].ToString());

                        string role = reader["role"].ToString();

                        reader.Close(); // Close the reader before redirecting

                        if (role == "user")
                            return RedirectToAction("Domestic", "Destination");

                        if (role == "admin")
                            return RedirectToAction("Dashboard", "Admin");
                    }

                    ViewBag.ErrorMessage = "Invalid email or password.";
                    return View("LoginPage", model);
                }
            }
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LoginPage");
        }
    }
}
