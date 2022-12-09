using Microsoft.AspNetCore.Mvc;

namespace shuttleasy.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
