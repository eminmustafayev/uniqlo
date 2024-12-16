using Microsoft.AspNetCore.Mvc;

namespace uniqilo.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
