using Microsoft.AspNetCore.Mvc;

namespace TennisCoach.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult ErrorMessage()
        {
            return View();
        }
    }
}
