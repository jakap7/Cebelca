using Cebelica.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Cebelica.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        private readonly CebelicaDatabase _context;

        public HomeController(CebelicaDatabase context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var products = _context.Products.Where(p => p.IsActive).ToList(); // Products passed to home/index for partial view!
            return View(products);
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
    }
}
