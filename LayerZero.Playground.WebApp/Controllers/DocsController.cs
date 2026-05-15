using Microsoft.AspNetCore.Mvc;

namespace LayerZero.Playground.WebApp.Controllers
{
    public class DocsController : Controller
    {
        public IActionResult Introduction() => View();
        public IActionResult Setup() => View();
        public IActionResult Configuration() => View();
        public IActionResult Benchmark() => View();
        public IActionResult ChangeLogs() => View();
    }
}
