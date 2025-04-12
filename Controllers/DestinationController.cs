using Microsoft.AspNetCore.Mvc;
using TheRoyalTourism.Models;
using Microsoft.Data.SqlClient;
using System.Text;

namespace TheRoyalTourism.Controllers
{
    public class DestinationController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        private readonly string _connectionString;

        public DestinationController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        //Domestic destinations
        public IActionResult Domestic()
        {
            var model = new AdminDataTablesViewModel
            {
                Destinations = new List<DestinationDisplayModel>(),
            };
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM destinations WHERE dtype='Domestic'", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Destinations.Add(new DestinationDisplayModel
                    {
                        Did = (int)reader["did"],
                        Dname = reader["dname"].ToString(),
                        Dimg = reader["dimg"].ToString(),
                    });
                }
                reader.Close();
            }
            return View(model);
        }


        //International destinations
        public IActionResult International()
        {
            var model = new AdminDataTablesViewModel
            {
                Destinations = new List<DestinationDisplayModel>(),
            };
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT * FROM destinations WHERE dtype='International'", conn);
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    model.Destinations.Add(new DestinationDisplayModel
                    {
                        Did = (int)reader["did"],
                        Dname = reader["dname"].ToString(),
                        Dimg = reader["dimg"].ToString(),
                    });
                }
                reader.Close();
            }
            return View(model);
        }

        // Packages
        public IActionResult PackagesPage(int id)
        {
            var model = new AdminDataTablesViewModel
            {
                Destinations = new List<DestinationDisplayModel>(),
                Packages = new List<PackageDisplayModel>(),
                Foods = new List<FoodDisplayModel>(),
                Activities = new List<ActivityDisplayModel>(),
                Places = new List<PlaceDisplayModel>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Packages
                var packageCmd = new SqlCommand("SELECT * FROM packages WHERE did = @did", conn);
                packageCmd.Parameters.AddWithValue("@did", id);
                using (var reader = packageCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Packages.Add(new PackageDisplayModel
                        {
                            Pid = (int)reader["pid"],
                            Pname = reader["pname"].ToString(),
                            Plocation = reader["plocation"].ToString(),
                            Pprice = Convert.ToDecimal(reader["pprice"]),
                            Pimg = reader["pimg"].ToString(),
                            Pday = (int)reader["pday"],
                            Package_Type = reader["package_type"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Foods
                var foodCmd = new SqlCommand("SELECT * FROM foods WHERE did = @did", conn);
                foodCmd.Parameters.AddWithValue("@did", id);
                using (var reader = foodCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Foods.Add(new FoodDisplayModel
                        {
                            Fid = (int)reader["fid"],
                            Fdetail = reader["fdetail"].ToString(),
                            Flocation = reader["flocation"].ToString(),
                            Fimg = reader["fimg"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Activities
                var activityCmd = new SqlCommand("SELECT * FROM activities WHERE did = @did", conn);
                activityCmd.Parameters.AddWithValue("@did", id);
                using (var reader = activityCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Activities.Add(new ActivityDisplayModel
                        {
                            Aid = (int)reader["aid"],
                            Adetail = reader["adetail"].ToString(),
                            Alocation = reader["alocation"].ToString(),
                            Aactivity = reader["aactivity"].ToString(),
                            Aimg = reader["aimg"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Places
                var placeCmd = new SqlCommand("SELECT * FROM places WHERE did = @did", conn);
                placeCmd.Parameters.AddWithValue("@did", id);
                using (var reader = placeCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Places.Add(new PlaceDisplayModel
                        {
                            Pid = Convert.ToInt32(reader["pl_id"]),
                            Pdetail = reader["pl_detail"].ToString(),
                            Ptime = reader["pl_time"].ToString(),
                            Plocation = reader["pl_location"].ToString(),
                            Pimg = reader["pl_img"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Destination
                var destCmd = new SqlCommand("SELECT * FROM destinations WHERE did = @did", conn);
                destCmd.Parameters.AddWithValue("@did", id);
                using (var reader = destCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Destinations.Add(new DestinationDisplayModel
                        {
                            Dname = reader["dname"].ToString()
                        });
                    }
                    reader.Close();
                }
            }

            return View(model);
        }


        // TourDetails
        public IActionResult TourDetailsPage(int id)
        {
            var model = new AdminDataTablesViewModel
            {
                Tour = new List<TourDisplayModel>(),
                Itineraries = new List<ItineraryModel>(),
                Packages = new List<PackageDisplayModel>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Fix: get package by pid instead of did
                var packageCmd = new SqlCommand("SELECT * FROM packages WHERE pid = @pid", conn);
                packageCmd.Parameters.AddWithValue("@pid", id);
                using (var reader = packageCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Packages.Add(new PackageDisplayModel
                        {
                            Pid = (int)reader["pid"],
                            Pprice = Convert.ToDecimal(reader["pprice"]),
                        });
                    }
                    reader.Close();
                }

                // Get Tour Details for this package
                var cmd = new SqlCommand("SELECT * FROM tourdetails WHERE pid = @pid", conn);
                cmd.Parameters.AddWithValue("@pid", id);
                List<int> tourIds = new List<int>();

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tid = (int)reader["tid"];
                        tourIds.Add(tid);

                        model.Tour.Add(new TourDisplayModel
                        {
                            Tid = tid,
                            Tname = reader["tname"].ToString(),
                            Tday = (int)reader["tday"],
                            Tpickup = reader["tpickup"].ToString(),
                            Timg1 = reader["timg1"].ToString(),
                            Timg2 = reader["timg2"].ToString(),
                            Timg3 = reader["timg3"].ToString(),
                            Timg4 = reader["timg4"].ToString(),
                            Toverview = reader["toverview"].ToString(),
                            Thighlights = reader["thighlights"].ToString()
                        });
                    }
                }

                // Get Itineraries
                foreach (var tid in tourIds)
                {
                    var itinCmd = new SqlCommand("SELECT * FROM itineraries WHERE tid = @tid", conn);
                    itinCmd.Parameters.AddWithValue("@tid", tid);

                    using (var reader = itinCmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            model.Itineraries.Add(new ItineraryModel
                            {
                                Iid = (int)reader["iid"],
                                Iday1 = reader["iday1"].ToString(),
                                Iday2 = reader["iday2"].ToString(),
                                Iday3 = reader["iday3"].ToString(),
                                Iday4 = reader["iday4"].ToString(),
                                Iday5 = reader["iday5"].ToString(),
                                Iday6 = reader["iday6"].ToString(),
                                Iday7 = reader["iday7"].ToString(),
                                Tid = tid
                            });
                        }
                    }
                }
            }

            return View(model);
        }





        // AnyFestival
        public IActionResult AnyFestival()
        {
            return View();
        }

    }
}
