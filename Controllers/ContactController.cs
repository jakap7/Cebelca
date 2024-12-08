using Microsoft.AspNetCore.Mvc;

namespace Cebelica.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
