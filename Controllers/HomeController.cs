using FoodPenguin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FoodPenguin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        // Page -----------------------------------

        public IActionResult GetResult()
        {
            return View();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Main()
        {
            return View();
        }
        public IActionResult History()
        {
            return View();
        }

        public IActionResult Order_Progress_Step_1()
        {
            return View();
        }

        public IActionResult Order_Progress_Step_2()
        {
            return View();
        }

        public IActionResult Order_Progress_Step_3()
        {
            return View();
        }

        public IActionResult Order_Progress_Step_3_1()
        {
            return View();
        }

        public IActionResult OrderAdd()
        {
            return View();
        }

        public IActionResult OrderDetail()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Progress_OrderList()
        {
            return View();
        }

        public IActionResult Progress_Step_0_Cancel()
        {
            return View();
        }

        public IActionResult Progress_Step_4_Success()
        {
            return View();
        }

        public IActionResult Receive_Progress_Step_1()
        {
            return View();
        }

        public IActionResult Receive_Progress_Step_2()
        {
            return View();
        }

        public IActionResult Receive_Progress_Step_3()
        {
            return View();
        }

        public IActionResult ReceiveAdd()
        {
            return View();
        }

        public IActionResult ReceiveDetail()
        {
            return View();
        }

        public IActionResult Starter_1()
        {
            return View();
        }

        public IActionResult Starter_2()
        {
            return View();
        }

        public IActionResult Starter_3()
        {
            return View();
        }

        // End Page --------------------------------

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}