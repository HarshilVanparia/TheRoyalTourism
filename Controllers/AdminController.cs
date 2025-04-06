using Microsoft.AspNetCore.Mvc;
using TheRoyalTourism.Models;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using System.IO;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

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



        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult Forms()
        {
            AllModels model = new AllModels();

            // Fetch destinations for dropdown
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT did, dname FROM destinations";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.DestinationList.Add(new DestinationModel
                        {
                            Did = Convert.ToInt32(reader["did"]),
                            Dname = reader["dname"].ToString()
                        });
                    }
                }
            }

            return View(model);
        }



        [HttpPost]
        public IActionResult AddUser(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                k.StartsWith("DestinationModel") || k.StartsWith("PackageModel")).ToList())
            {
                ModelState.Remove(key);
            }

            var user = model.UserModel;

            var validationContext = new ValidationContext(user);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    ModelState.AddModelError($"UserModel.{validationResult.MemberNames.First()}", validationResult.ErrorMessage);
                }

                model.DestinationList = GetDestinations(); // if package dropdown exists
                return View("Forms", model);
            }

            // Your SQL insert logic
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO users (fullname, email, pnumber, password, role) VALUES (@Fullname, @Email, @Pnumber, @Password, @Role)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Fullname", model.UserModel.Fullname);
                    cmd.Parameters.AddWithValue("@Email", model.UserModel.Email);
                    cmd.Parameters.AddWithValue("@Pnumber", model.UserModel.Pnumber);
                    cmd.Parameters.AddWithValue("@Password", model.UserModel.Password);
                    cmd.Parameters.AddWithValue("@Role", model.UserModel.Role ?? "user");

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["Success"] = "User added successfully!";
            return RedirectToAction("Forms");
        }


        [HttpPost]
        public IActionResult AddDestination(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                k.StartsWith("UserModel") || k.StartsWith("PackageModel"))  .ToList())
            {
                ModelState.Remove(key);
            }

            var destination = model.DestinationModel;

            var validationContext = new ValidationContext(destination);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(destination, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    ModelState.AddModelError($"DestinationModel.{validationResult.MemberNames.First()}", validationResult.ErrorMessage);
                }

                model.DestinationList = GetDestinations(); // in case your form needs dropdown
                return View("Forms", model);
            }

            // Save image
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = destination.Dimg.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                destination.Dimg.CopyTo(stream);
            }

            // Insert into DB
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO destinations (dname, dimg, dtype) VALUES (@Dname, @Dimg, @Dtype)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Dname", destination.Dname);
                    cmd.Parameters.AddWithValue("@Dimg", fileName);
                    cmd.Parameters.AddWithValue("@Dtype", destination.Dtype);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["DestinationSuccess"] = "Destination added successfully!";
            return RedirectToAction("Forms");
        }


        [HttpPost]
        public IActionResult AddPackage(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                k.StartsWith("UserModel") || k.StartsWith("DestinationModel")) .ToList())
            {
                ModelState.Remove(key);
            }

            var package = model.PackageModel;

            var validationContext = new ValidationContext(package);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(package, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    ModelState.AddModelError($"PackageModel.{result.MemberNames.First()}", result.ErrorMessage);
                }

                model.DestinationList = GetDestinations();
                return View("Forms", model);
            }

            // Save image
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = package.Pimg.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                package.Pimg.CopyTo(stream);
            }

            // Insert into DB
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO packages (pname, plocation, pprice, pimg, pday, did, package_type) " +
                                     "VALUES (@Pname, @Plocation, @Pprice, @Pimg, @Pday, @Did, @PackageType)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Pname", package.Pname);
                    cmd.Parameters.AddWithValue("@Plocation", package.Plocation);
                    cmd.Parameters.AddWithValue("@Pprice", package.Pprice);
                    cmd.Parameters.AddWithValue("@Pimg", fileName);
                    cmd.Parameters.AddWithValue("@Pday", package.Pday);
                    cmd.Parameters.AddWithValue("@Did", package.Did);
                    cmd.Parameters.AddWithValue("@PackageType", package.Package_Type);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["PackageSuccess"] = "Package added successfully!";
            return RedirectToAction("Forms");
        }

        private List<DestinationModel> GetDestinations()
        {
            var list = new List<DestinationModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT did, dname FROM destinations";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new DestinationModel
                        {
                            Did = Convert.ToInt32(reader["did"]),
                            Dname = reader["dname"].ToString()
                        });
                    }
                }
            }
            return list;
        }



        [HttpPost]
        public IActionResult AddActivity(AllModels model)
        {
            // Remove validation for unrelated forms
            foreach (var key in ModelState.Keys.Where(k =>
                k.StartsWith("UserModel") || k.StartsWith("DestinationModel") || k.StartsWith("PackageModel")).ToList())
            {
                ModelState.Remove(key);
            }

            var activity = model.ActivityModel;

            var validationContext = new ValidationContext(activity);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(activity, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    ModelState.AddModelError($"ActivityModel.{result.MemberNames.First()}", result.ErrorMessage);
                }

                model.DestinationList = GetDestinations();
                return View("Forms", model);
            }

            // ✅ Save the image
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = activity.Aimg.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                activity.Aimg.CopyTo(stream);
            }

            // ✅ Insert into DB
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO activities (adetail, atime, alocation, aactivity, aimg, did) " +
                                     "VALUES (@Adetail, @Atime, @Alocation, @Aactivity, @Aimg, @Did)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Adetail", activity.Adetail);
                    cmd.Parameters.AddWithValue("@Atime", activity.Atime);
                    cmd.Parameters.AddWithValue("@Alocation", activity.Alocation);
                    cmd.Parameters.AddWithValue("@Aactivity", activity.Aactivity);
                    cmd.Parameters.AddWithValue("@Aimg", fileName);
                    cmd.Parameters.AddWithValue("@Did", activity.Did);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["ActivitySuccess"] = "Activity added successfully!";
            return RedirectToAction("Forms");
        }


        [HttpPost]
        public IActionResult AddPlace(AllModels model)
        {
            // Clear other validations
            foreach (var key in ModelState.Keys.Where(k =>
                k.StartsWith("UserModel") || k.StartsWith("DestinationModel") ||
                k.StartsWith("PackageModel") || k.StartsWith("ActivityModel")).ToList())
            {
                ModelState.Remove(key);
            }

            var place = model.PlaceModel;
            var validationContext = new ValidationContext(place);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(place, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    ModelState.AddModelError($"PlaceModel.{result.MemberNames.First()}", result.ErrorMessage);
                }

                model.DestinationList = GetDestinations();
                return View("Forms", model);
            }

            // Save image
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = place.Pl_Img.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                place.Pl_Img.CopyTo(stream);
            }

            // Insert into DB
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO places (pl_detail, pl_time, pl_location, pl_img, did)
                               VALUES (@Detail, @Time, @Location, @Img, @Did)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Detail", place.Pl_Detail);
                    cmd.Parameters.AddWithValue("@Time", place.Pl_Time);
                    cmd.Parameters.AddWithValue("@Location", place.Pl_Location);
                    cmd.Parameters.AddWithValue("@Img", fileName);
                    cmd.Parameters.AddWithValue("@Did", place.Did);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["PlaceSuccess"] = "Place added successfully!";
            return RedirectToAction("Forms");
        }





    }
}
