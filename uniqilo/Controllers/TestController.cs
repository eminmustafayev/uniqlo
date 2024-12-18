using Microsoft.AspNetCore.Mvc;

namespace uniqilo.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
