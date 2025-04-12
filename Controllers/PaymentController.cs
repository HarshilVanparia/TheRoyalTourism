using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Razorpay.Api;

namespace TheRoyalTourism.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly RazorpayOptions _razorpayOptions;

        public PaymentController(IOptions<RazorpayOptions> razorpayOptions, IConfiguration config, IWebHostEnvironment env)
        {
            _razorpayOptions = razorpayOptions.Value;
            _config = config;
            _env = env;
        }

        [HttpPost]
        public IActionResult CreateOrder(decimal amount, int pid)
        {
            RazorpayClient client = new RazorpayClient(_razorpayOptions.Key, _razorpayOptions.Secret);

            Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", amount * 100 }, // amount in paise
            { "currency", "INR" },
            { "receipt", Guid.NewGuid().ToString() },
            { "payment_capture", "1" }
        };

            Order order = client.Order.Create(options);

            ViewBag.Key = _razorpayOptions.Key;
            ViewBag.OrderId = order["id"].ToString();
            ViewBag.Amount = amount;
            ViewBag.Pid = pid;

            return View("PaymentPage");
        }

        [HttpPost]
        public IActionResult PaymentSuccess(string razorpay_payment_id, string razorpay_order_id, string razorpay_signature, int pid, decimal amount)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetInt32("UserId")); // Assuming you store user ID in session

            using (SqlConnection con = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO payments (uid, pid, amount, transaction_id, payment_status) VALUES (@uid, @pid, @amount, @txid, 'Success')", con);
                cmd.Parameters.AddWithValue("@uid", uid);
                cmd.Parameters.AddWithValue("@pid", pid);
                cmd.Parameters.AddWithValue("@amount", amount);
                cmd.Parameters.AddWithValue("@txid", razorpay_payment_id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("PaymentSuccessPage");
        }
        public IActionResult PaymentSuccessPage()
        {
            return View();
        }
    }
}
