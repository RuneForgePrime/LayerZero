using Microsoft.AspNetCore.Mvc;

namespace LayerZero.Playground.WebApp.Controllers
{
    public class DemoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult JsAnimation()
        {
            return View();
        }
    }
}
