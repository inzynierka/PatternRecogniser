using Microsoft.AspNetCore.Mvc;

namespace PatternRecogniser.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
    }
}
