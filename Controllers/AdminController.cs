using Microsoft.AspNetCore.Mvc;
using TheRoyalTourism.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;

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

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        // Form Data Inserting
        [HttpPost]
        public IActionResult AddUser(User user)
        {
            if (!ModelState.IsValid)
            {
                return View("Forms", user); // Return form with validation errors
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Check if the email already exists
                string checkQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Email", user.Email);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        TempData["Error"] = "Email already exists!";
                        return RedirectToAction("Forms");
                    }
                }

                // Insert user into the database
                string insertQuery = "INSERT INTO users (fullname, email, pnumber, password, role, status) VALUES (@FullName, @Email, @PNumber, @Password,'user', @Status)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@PNumber", user.PNumber);
                    cmd.Parameters.AddWithValue("@Password", user.Password); // Consider hashing password
                    cmd.Parameters.AddWithValue("@Role", "user");
                    cmd.Parameters.AddWithValue("@Status", "Active"); // Default status

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Success"] = "User added successfully!";
            return RedirectToAction("Forms");
        }


        [HttpPost]
        public IActionResult AddDestinations(AllModels model, IFormFile DestinationImage)
        {
            // 🚨 Check if the file is missing
            if (DestinationImage == null || DestinationImage.Length == 0)
            {
                ModelState.AddModelError("Destination.DImg", "Please upload a valid image file.");
                return View("Forms", model);
            }

            // ✅ Validate File Type (only JPG, JPEG, PNG)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".svg", ".png" };
            var fileExtension = Path.GetExtension(DestinationImage.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("Destination.DImg", "Only JPG, JPEG, SVG, and PNG files are allowed.");
                return View("Forms", model);
            }

            // ✅ Validate File Size (Max 5MB)
            //if (DestinationImage.Length > 5 * 1024 * 1024) // 5MB limit
            //{
            //    ModelState.AddModelError("Destination.DImg", "File size must be less than 5MB.");
            //    return View("Forms", model);
            //}

            // ✅ Ensure the 'uploads' directory exists
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // ✅ Keep Original File Name
            string fileName = DestinationImage.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            // ✅ Save File to 'uploads' folder
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                DestinationImage.CopyTo(fileStream);
            }

            // ✅ Insert into Database
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO destinations (dname, dimg, dtype) VALUES (@DName, @DImg, @DType)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DName", model.Destination.DName);
                    cmd.Parameters.AddWithValue("@DImg", fileName); // Stores original file name
                    cmd.Parameters.AddWithValue("@DType", model.Destination.DType);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        TempData["msg"] = "Destination added successfully!";
                        return RedirectToAction("Dashboard");
                    }
                    else
                    {
                        TempData["error"] = "Failed to add destination.";
                        return View("Forms", model);
                    }
                }
            }
        }


    }
}
