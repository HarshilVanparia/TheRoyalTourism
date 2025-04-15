using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using TheRoyalTourism.Models;
using Microsoft.Data.SqlClient;

namespace TheRoyalTourism.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly string _connectionString;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        public IActionResult Index()
        {
            var model = new AdminDataTablesViewModel
            {
                Destinations = new List<DestinationDisplayModel>(),
                InternationalDestinations = new List<DestinationDisplayModel>()
            };

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // Domestic
                var cmd = new SqlCommand("SELECT * FROM destinations WHERE dtype='Domestic'", conn);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.Destinations.Add(new DestinationDisplayModel
                        {
                            Did = (int)reader["did"],
                            Dname = reader["dname"].ToString(),
                            Dimg = reader["dimg"].ToString(),
                        });
                    }
                }

                // International
                var intcmd = new SqlCommand("SELECT * FROM destinations WHERE dtype='International'", conn);
                using (var intreader = intcmd.ExecuteReader())
                {
                    while (intreader.Read())
                    {
                        model.InternationalDestinations.Add(new DestinationDisplayModel
                        {
                            Did = (int)intreader["did"],
                            Dname = intreader["dname"].ToString(),
                            Dimg = intreader["dimg"].ToString(),
                        });
                    }
                }
            }

            return View(model);

        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
