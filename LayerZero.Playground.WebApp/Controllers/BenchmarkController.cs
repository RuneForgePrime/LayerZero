using Microsoft.AspNetCore.Mvc;

namespace LayerZero.Playground.WebApp.Controllers
{
    public class BenchmarkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
