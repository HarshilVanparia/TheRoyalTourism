using Microsoft.AspNetCore.Mvc;
using TheRoyalTourism.Models;
using Microsoft.Data.SqlClient;
namespace TheRoyalTourism.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IConfiguration _config;

        public UserController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult UserProfilePage()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetInt32("UserId"));
            List<PurchaseModel> purchases = new List<PurchaseModel>();

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();

                string query = @"SELECT p.amount, pk.pname, pk.plocation, pk.pprice 
                                 FROM payments p
                                 JOIN packages pk ON p.pid = pk.pid
                                 WHERE p.uid = @uid";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@uid", uid);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        purchases.Add(new PurchaseModel
                        {
                            PackageName = reader["pname"].ToString(),
                            Location = reader["plocation"].ToString(),
                            Price = Convert.ToDecimal(reader["pprice"]),
                            AmountPaid = Convert.ToDecimal(reader["amount"])
                        });
                    }
                }
            }

            ViewBag.Purchases = purchases;
            return View();
        }
    }
}
