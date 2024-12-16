using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace uniqilo.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles="Admin")]
    public class DashBoard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
