using System.Diagnostics;
using Agri_Connect.Models;
using Microsoft.AspNetCore.Mvc;

namespace Agri_Connect.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
        [Route("Home/Error/{code}")]
        public IActionResult Error(int code)
        {
            ViewData["ErrorCode"] = code;

            switch (code)
            {
                case 404:
                    ViewData["ErrorMessage"] = "Sorry, the page you requested could not be found.";
                    break;
                case 500:
                    ViewData["ErrorMessage"] = "Sorry, something went wrong on the server.";
                    break;
                default:
                    ViewData["ErrorMessage"] = "An unexpected error occurred.";
                    break;
            }

            return View("CustomError");
        }
    }
}
