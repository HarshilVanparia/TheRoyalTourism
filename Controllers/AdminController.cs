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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Documents;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using ServiceStack;

namespace TheRoyalTourism.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private readonly string _connectionString;

        public AdminController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "admin")
            {
                return RedirectToAction("LoginPage", "Register");
            }
            var model = new DashboardViewModel();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                model.TotalDestinations = GetCount(conn, "SELECT COUNT(*) FROM destinations");
                model.TotalDomesticDestinations = GetCount(conn, "SELECT COUNT(*) FROM destinations WHERE dtype= 'Domestic'");
                model.TotalInternationalDestinations = GetCount(conn, "SELECT COUNT(*) FROM destinations WHERE dtype= 'International'");
                model.TotalPackages = GetCount(conn, "SELECT COUNT(*) FROM packages");
                model.TotalDomesticPackages = GetCount(conn, "SELECT COUNT(*) FROM packages WHERE package_type = 'regular'");
                model.TotalInternationalPackages = GetCount(conn, "SELECT COUNT(*) FROM packages WHERE package_type = 'festival'");
                model.TotalUsers = GetCount(conn, "SELECT COUNT(*) FROM users");
                model.TotalTours = GetCount(conn, "SELECT COUNT(*) FROM tourdetails");
                model.TotalItineraries = GetCount(conn, "SELECT COUNT(*) FROM itineraries");
                model.TotalActivities = GetCount(conn, "SELECT COUNT(*) FROM activities");
                model.TotalFoods = GetCount(conn, "SELECT COUNT(*) FROM foods");
                model.TotalPlaces = GetCount(conn, "SELECT COUNT(*) FROM places");
            }

            return View(model);
        }
        private int GetCount(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return (int)cmd.ExecuteScalar();
            }
        }


        public IActionResult DataTables()
        {
            var model = new AdminDataTablesViewModel
            {
                Users = new List<UserModel>(),
                Itineraries = new List<ItineraryModel>(),
                Activities = new List<ActivityDisplayModel>(),
                Foods = new List<FoodDisplayModel>(),
                Destinations = new List<DestinationDisplayModel>(),
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Users
                var cmd = new SqlCommand("SELECT * FROM users WHERE role = 'user'", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Users.Add(new UserModel
                    {
                        Fullname = reader["fullname"].ToString(),     // Adjust based on actual column name
                        Email = reader["email"].ToString(),
                        Pnumber = reader["pnumber"].ToString(),
                        Password = reader["password"].ToString()
                    });
                }
                reader.Close();

                // Destinations
                cmd = new SqlCommand("SELECT * FROM destinations", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Destinations.Add(new DestinationDisplayModel
                    {
                        Did = (int)reader["did"],
                        Dname = reader["dname"].ToString(),
                        Dimg = reader["dimg"].ToString(),
                        Dtype = reader["dtype"].ToString()
                    });
                }
                reader.Close();

                // Foods
                cmd = new SqlCommand("SELECT f.*, d.dname FROM foods f JOIN destinations d ON f.did = d.did", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Foods.Add(new FoodDisplayModel
                    {
                        Fdetail = reader["fdetail"].ToString(),
                        Flocation = reader["flocation"].ToString(),
                        Fimg = reader["fimg"].ToString(),
                        Dname = reader["dname"].ToString()
                    });
                }
                reader.Close();

                // Activities
                cmd = new SqlCommand("SELECT a.*, d.dname FROM activities a JOIN destinations d ON a.did = d.did", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Activities.Add(new ActivityDisplayModel
                    {
                        Adetail = reader["adetail"].ToString(),
                        Atime = reader["atime"].ToString(),
                        Alocation = reader["alocation"].ToString(),
                        Aactivity = reader["aactivity"].ToString(),
                        Aimg = reader["aimg"].ToString(),
                        Dname = reader["dname"].ToString()
                    });
                }
                reader.Close();

                // Itineraries (Join with tour name)
                cmd = new SqlCommand("SELECT i.*, t.tname FROM itineraries i JOIN tourdetails t ON i.tid = t.tid", conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Itineraries.Add(new ItineraryModel
                    {
                        Iday1 = reader["iday1"].ToString(),
                        Iday2 = reader["iday2"].ToString(),
                        Iday3 = reader["iday3"].ToString(),
                        Iday4 = reader["iday4"].ToString(),
                        Iday5 = reader["iday5"].ToString(),
                        Iday6 = reader["iday6"].ToString(),
                        Iday7 = reader["iday7"].ToString(),
                    });
                }
                reader.Close();
            }

            return View(model);
        }


        public IActionResult Packages()
        {
            var model = new AdminDataTablesViewModel
            {
                Packages = new List<PackageDisplayModel>(),
            };
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM packages", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Packages.Add(new PackageDisplayModel
                    {
                        Pname = reader["pname"].ToString(),
                        Plocation = reader["plocation"].ToString(),
                        Pprice = (decimal)reader["pprice"],
                        Pimg = reader["pimg"].ToString(),
                        Pday = (int)reader["pday"],
                        Package_Type = reader["package_type"].ToString(),
                        Did = (int)reader["did"],
                    });
                }
                reader.Close();
            }
            return View(model);
        }
        public IActionResult Details()
        {
            var model = new AdminDataTablesViewModel
            {
                Tour = new List<TourDisplayModel>(),
            };
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM tourdetails", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Tour.Add(new TourDisplayModel
                    {
                        Tid = (int)reader["tid"],
                        Tname = reader["tname"].ToString(),
                        Tday = (int)reader["tday"],
                        Tpickup = reader["tpickup"].ToString(),
                        Timg1 = reader["timg1"].ToString(),
                        Timg2 = reader["timg2"].ToString(),
                        Timg3 = reader["timg3"].ToString(),
                        Timg4 = reader["timg4"].ToString(),
                        Toverview = reader["toverview"].ToString(),
                        Thighlights = reader["thighlights"].ToString(),
                        Pid = (int)reader["pid"],
                    });
                }
                reader.Close();
            }
            return View(model);
        }
        public IActionResult AdminProfile()
        {
            return View();
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
                string packageQuery = "SELECT pid, pname FROM packages";
                using (SqlCommand cmd = new SqlCommand(packageQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.PackageList.Add(new PackageModel
                        {
                            Pid = Convert.ToInt32(reader["pid"]),
                            Pname = reader["pname"].ToString()
                        });
                    }
                }
                string tourQuery = "SELECT tid, tname FROM tourdetails";
                using (SqlCommand cmd = new SqlCommand(tourQuery, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.TourList.Add(new TourModel
                        {
                            Tid = Convert.ToInt32(reader["tid"]),
                            Tname = reader["tname"].ToString()
                        });
                    }
                }
            }

            return View(model);
        }


        //adduser
        [HttpPost]
        public IActionResult AddUser(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                   k.StartsWith("DestinationModel") ||
                   k.StartsWith("PackageModel") ||
                   k.StartsWith("ActivityModel") ||
                   k.StartsWith("PlaceModel") ||
                   k.StartsWith("FoodModel") ||
                   k.StartsWith("TourModel") ||
                   k.StartsWith("ItineraryModel")
               ).ToList())
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


        //adddestination
        [HttpPost]
        public IActionResult AddDestination(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                  k.StartsWith("PackageModel") ||
                  k.StartsWith("ActivityModel") ||
                  k.StartsWith("UserModel") ||
                  k.StartsWith("PlaceModel") ||
                  k.StartsWith("FoodModel") ||
                  k.StartsWith("TourModel") ||
                  k.StartsWith("ItineraryModel")
              ).ToList())
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


        //addpackage
        [HttpPost]
        public IActionResult AddPackage(AllModels model)
        {
            // Remove other form validations
            foreach (var key in ModelState.Keys.Where(k =>
                  k.StartsWith("DestinationModel") ||
                  k.StartsWith("ActivityModel") ||
                  k.StartsWith("PlaceModel") ||
                  k.StartsWith("UserModel") ||
                  k.StartsWith("FoodModel") ||
                  k.StartsWith("TourModel") ||
                  k.StartsWith("ItineraryModel")
              ).ToList())
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


        //addactivity
        [HttpPost]
        public IActionResult AddActivity(AllModels model)
        {
            // Remove validation for unrelated forms
            foreach (var key in ModelState.Keys.Where(k =>
                  k.StartsWith("DestinationModel") ||
                  k.StartsWith("PackageModel") ||
                  k.StartsWith("PlaceModel") ||
                  k.StartsWith("UserModel") ||
                  k.StartsWith("FoodModel") ||
                  k.StartsWith("TourModel") ||
                  k.StartsWith("ItineraryModel")
              ).ToList())
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


        //addplace
        [HttpPost]
        public IActionResult AddPlace(AllModels model)
        {
            // Clear other validations
            foreach (var key in ModelState.Keys.Where(k =>
                   k.StartsWith("DestinationModel") ||
                   k.StartsWith("PackageModel") ||
                   k.StartsWith("ActivityModel") ||
                   k.StartsWith("UserModel") ||
                   k.StartsWith("FoodModel") ||
                   k.StartsWith("TourModel") ||
                   k.StartsWith("ItineraryModel")
               ).ToList())
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


        //addfood
        [HttpPost]
        public IActionResult AddFood(AllModels model)
        {
            // Remove validation keys for other models
            foreach (var key in ModelState.Keys.Where(k =>
                  k.StartsWith("DestinationModel") ||
                  k.StartsWith("PackageModel") ||
                  k.StartsWith("ActivityModel") ||
                  k.StartsWith("UserModel") ||
                  k.StartsWith("PlaceModel") ||
                  k.StartsWith("TourModel") ||
                  k.StartsWith("ItineraryModel")
              ).ToList())
            {
                ModelState.Remove(key);
            }

            var food = model.FoodModel;

            var validationContext = new ValidationContext(food);
            var validationResults = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(food, validationContext, validationResults, true);

            if (!isValid)
            {
                foreach (var result in validationResults)
                {
                    ModelState.AddModelError($"FoodModel.{result.MemberNames.First()}", result.ErrorMessage);
                }

                model.DestinationList = GetDestinations();
                return View("Forms", model);
            }

            // Save image
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = food.Fimg.FileName;
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                food.Fimg.CopyTo(stream);
            }

            // Insert into DB
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO foods (fdetail, flocation, fimg, did) VALUES (@Fdetail, @Flocation, @Fimg, @Did)";
                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Fdetail", food.Fdetail);
                    cmd.Parameters.AddWithValue("@Flocation", food.Flocation);
                    cmd.Parameters.AddWithValue("@Fimg", fileName);
                    cmd.Parameters.AddWithValue("@Did", food.Did);
                    cmd.ExecuteNonQuery();
                }
            }

            TempData["FoodSuccess"] = "Food added successfully!";
            return RedirectToAction("Forms");
        }




        //addtour
        [HttpPost]
        public IActionResult AddTour(AllModels model)
        {
            // Clear unrelated ModelState
            foreach (var key in ModelState.Keys.Where(k =>
                  k.StartsWith("DestinationModel") ||
                  k.StartsWith("PackageModel") ||
                  k.StartsWith("ActivityModel") ||
                  k.StartsWith("UserModel") ||
                  k.StartsWith("PlaceModel") ||
                  k.StartsWith("FoodModel") ||
                  k.StartsWith("ItineraryModel")
              ).ToList())
            {
                ModelState.Remove(key);
            }

            var tour = model.TourModel;

            var context = new ValidationContext(tour);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(tour, context, results, true);

            if (!isValid)
            {
                foreach (var res in results)
                {
                    ModelState.AddModelError($"TourModel.{res.MemberNames.First()}", res.ErrorMessage);
                }

                model.PackageList = GetPackages();
                return View("Forms", model);
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string SaveFile(IFormFile file)
            {
                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return file.FileName;
            }

            string img1 = SaveFile(tour.Timg1);
            string img2 = SaveFile(tour.Timg2);
            string img3 = SaveFile(tour.Timg3);
            string img4 = SaveFile(tour.Timg4);

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO tourdetails 
            (tname, tday, tpickup, timg1, timg2, timg3, timg4, toverview, thighlights, pid) 
            VALUES 
            (@Tname, @Tday, @Tpickup, @Timg1, @Timg2, @Timg3, @Timg4, @Toverview, @Thighlights, @Pid)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Tname", tour.Tname);
                    cmd.Parameters.AddWithValue("@Tday", tour.Tday);
                    cmd.Parameters.AddWithValue("@Tpickup", tour.Tpickup);
                    cmd.Parameters.AddWithValue("@Timg1", img1);
                    cmd.Parameters.AddWithValue("@Timg2", img2);
                    cmd.Parameters.AddWithValue("@Timg3", img3);
                    cmd.Parameters.AddWithValue("@Timg4", img4);
                    cmd.Parameters.AddWithValue("@Toverview", tour.Toverview);
                    cmd.Parameters.AddWithValue("@Thighlights", tour.Thighlights);
                    cmd.Parameters.AddWithValue("@Pid", tour.Pid);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["TourSuccess"] = "Tour added successfully!";
            return RedirectToAction("Forms");
        }

        private List<PackageModel> GetPackages()
        {
            var list = new List<PackageModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT pid, pname FROM packages", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PackageModel
                        {
                            Pid = Convert.ToInt32(reader["pid"]),
                            Pname = reader["pname"].ToString()
                        });
                    }
                }
            }
            return list;
        }


        //AddItinerary
        [HttpPost]
        public IActionResult AddItinerary(AllModels model)
        {
            // Clear unrelated model keys
            foreach (var key in ModelState.Keys.Where(k =>
                       k.StartsWith("DestinationModel") ||
                       k.StartsWith("PackageModel") ||
                       k.StartsWith("ActivityModel") ||
                       k.StartsWith("UserModel") ||
                       k.StartsWith("PlaceModel") ||
                       k.StartsWith("FoodModel") ||
                       k.StartsWith("TourModel")
                   ).ToList())
            {
                ModelState.Remove(key);
            }

            var itinerary = model.ItineraryModel;

            var context = new ValidationContext(itinerary);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(itinerary, context, results, true);

            if (!isValid)
            {
                foreach (var res in results)
                {
                    ModelState.AddModelError($"ItineraryModel.{res.MemberNames.First()}", res.ErrorMessage);
                }

                model.TourList = GetTours();
                return View("Forms", model);
            }

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string insertQuery = @"INSERT INTO itineraries 
        (iday1, iday2, iday3, iday4, iday5, iday6, iday7, tid) 
        VALUES 
        (@Iday1, @Iday2, @Iday3, @Iday4, @Iday5, @Iday6, @Iday7, @Tid)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Iday1", itinerary.Iday1 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday2", itinerary.Iday2 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday3", itinerary.Iday3 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday4", itinerary.Iday4 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday5", itinerary.Iday5 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday6", itinerary.Iday6 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Iday7", itinerary.Iday7 ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Tid", itinerary.Tid);

                    cmd.ExecuteNonQuery();
                }
            }

            TempData["ItinerarySuccess"] = "Itinerary added successfully!";
            return RedirectToAction("Forms");
        }


        private List<TourModel> GetTours()
        {
            var list = new List<TourModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT tid, tname FROM tourdetails", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new TourModel
                        {
                            Tid = Convert.ToInt32(reader["tid"]),
                            Tname = reader["tname"].ToString()
                        });
                    }
                }
            }
            return list;
        }





    }
}


/*

-- CREATE DATABASE TheRoyalTourism;
-- GO
-- USE TheRoyalTourism;
-- GO

-- Table: Users (For Authentication and Role Management)
CREATE TABLE users (
    uid INT PRIMARY KEY IDENTITY(1,1),
    fullname VARCHAR(255) NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    pnumber NUMERIC(20) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) DEFAULT 'user' CHECK (role IN ('user', 'admin')),
    created_at DATETIME DEFAULT GETDATE()
);

-- Table: Destinations
CREATE TABLE destinations (
    did INT PRIMARY KEY IDENTITY(1,1),
    dname NVARCHAR(255) UNIQUE NOT NULL,
    dimg NVARCHAR(255) NOT NULL,
    dtype NVARCHAR(50) NOT NULL
);

-- Table: Packages (Unified for Regular and Festival Packages)
CREATE TABLE packages (
    pid INT PRIMARY KEY IDENTITY(1,1),
    pname NVARCHAR(255) NOT NULL,
    plocation NVARCHAR(255) NOT NULL,
    pprice DECIMAL(10,2) NOT NULL CHECK (pprice >= 0),
    pimg NVARCHAR(255) NOT NULL,
    pday INT NOT NULL CHECK (pday >= 0),
    did INT NOT NULL,
    package_type NVARCHAR(50) NOT NULL DEFAULT 'regular' CHECK (package_type IN ('regular', 'festival')),
    CONSTRAINT FK_Packages_Destinations FOREIGN KEY (did) REFERENCES destinations(did) ON DELETE CASCADE
);

-- Table: Activities (Linked to Destinations)
CREATE TABLE activities (
    aid INT PRIMARY KEY IDENTITY(1,1),
    adetail NVARCHAR(MAX) NOT NULL,
    atime NVARCHAR(255) NOT NULL,
    alocation NVARCHAR(255) NOT NULL,
    aactivity NVARCHAR(255) NOT NULL,
    aimg NVARCHAR(255) NOT NULL,
    did INT NOT NULL,
    CONSTRAINT FK_Activities_Destinations FOREIGN KEY (did) REFERENCES destinations(did) ON DELETE CASCADE
);
CREATE INDEX idx_activities_did ON activities(did);

-- Table: Places (Linked to Destinations)
CREATE TABLE places (
    pl_id INT PRIMARY KEY IDENTITY(1,1),
    pl_detail NVARCHAR(MAX) NOT NULL,
    pl_time NVARCHAR(255) NOT NULL,
    pl_location NVARCHAR(255) NOT NULL,
    pl_img NVARCHAR(255) NOT NULL,
    did INT NOT NULL,
    CONSTRAINT FK_Places_Destinations FOREIGN KEY (did) REFERENCES destinations(did) ON DELETE CASCADE
);
CREATE INDEX idx_places_did ON places(did);

-- Table: Foods (Linked to Destinations)
CREATE TABLE foods (
    fid INT PRIMARY KEY IDENTITY(1,1),
    fdetail NVARCHAR(MAX) NOT NULL,
    flocation NVARCHAR(255) NOT NULL,
    fimg NVARCHAR(255) NOT NULL,
    did INT NOT NULL,
    CONSTRAINT FK_Foods_Destinations FOREIGN KEY (did) REFERENCES destinations(did) ON DELETE CASCADE
);
CREATE INDEX idx_foods_did ON foods(did);

-- Table: Tour Details (Linked to Packages)
CREATE TABLE tourdetails (
    tid INT PRIMARY KEY IDENTITY(1,1),
    tname NVARCHAR(255) NOT NULL,
    tday INT NOT NULL CHECK (tday >= 0),
    tpickup NVARCHAR(255) NOT NULL,
    timg1 NVARCHAR(255) NOT NULL,
    timg2 NVARCHAR(255) NOT NULL,
    timg3 NVARCHAR(255) NOT NULL,
    timg4 NVARCHAR(255) NOT NULL,
    toverview NVARCHAR(MAX) NOT NULL,
    thighlights NVARCHAR(MAX) NOT NULL,
    pid INT NOT NULL,
    CONSTRAINT FK_TourDetails_Packages FOREIGN KEY (pid) REFERENCES packages(pid) ON DELETE CASCADE
);

-- Table: Tour Images (Added for Flexibility, Optional)
CREATE TABLE tour_images (
    tiid INT PRIMARY KEY IDENTITY(1,1),
    tour_img NVARCHAR(255) NOT NULL,
    tid INT NOT NULL,
    CONSTRAINT FK_TourImages_TourDetails FOREIGN KEY (tid) REFERENCES tourdetails(tid) ON DELETE CASCADE
);

-- Table: Itineraries (Linked to Tour Details)
CREATE TABLE itineraries (
    iid INT PRIMARY KEY IDENTITY(1,1),
    iday1 NVARCHAR(MAX) NULL,
    iday2 NVARCHAR(MAX) NULL,
    iday3 NVARCHAR(MAX) NULL,
    iday4 NVARCHAR(MAX) NULL,
    iday5 NVARCHAR(MAX) NULL,
    iday6 NVARCHAR(MAX) NULL,
    iday7 NVARCHAR(MAX) NULL,
    tid INT NOT NULL,
    CONSTRAINT FK_Itineraries_TourDetails FOREIGN KEY (tid) REFERENCES tourdetails(tid) ON DELETE CASCADE
);

-- Table: Payments (For Razorpay Payment Gateway Integration)
CREATE TABLE payments (
    payid INT PRIMARY KEY IDENTITY(1,1),
    uid INT NOT NULL,
    pid INT NOT NULL,
    amount DECIMAL(10,2) NOT NULL CHECK (amount >= 0),
    transaction_id NVARCHAR(255) UNIQUE NOT NULL,
    payment_status NVARCHAR(50) NOT NULL,
    created_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Payments_Users FOREIGN KEY (uid) REFERENCES users(uid) ON DELETE CASCADE,
    CONSTRAINT FK_Payments_Packages FOREIGN KEY (pid) REFERENCES packages(pid) ON DELETE CASCADE
);
CREATE INDEX idx_payments_transaction_id ON payments(transaction_id);

this is database


functionalities :
1. Upload Destination 
2. Upload Packages for particular destinations and that can be more for example in Goa destination we can 6 packages then can be more.
4. Each Destination have these 3 common details activity, places, food of that destination.
5. Each package contain tour details for example there is package Goa Bliss Package so only few details are shown but insde that package there is tour deails of that whole package. 
6. Upload Package for any festival. 
7. Razor pay pament getway

Project technology is ASP.Net Core MVC

the database is loacal not migration{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\Ghost\\Documents\\theroyaltourism.mdf;Integrated Security=True;Connect Timeout=30"
  }
}

remember this well and understand the connection between database and functionalities completely. 
so we can move on to the next stage
 */